#!/usr/bin/env python3
"""Bootstrap through Unity itself; never invent settings, package locks or scene YAML."""

import argparse
import json
from pathlib import Path
import plistlib
import shutil
import subprocess
import sys
import tarfile

ROOT = Path(__file__).resolve().parents[1]
VERSION = (ROOT / ".unity-version").read_text().strip()
DEFAULT_EDITOR = Path(f"/Applications/Unity/Hub/Editor/{VERSION}/Unity.app/Contents/MacOS/Unity")


def run_unity(editor, *arguments):
    subprocess.run([str(editor), "-batchmode", "-quit", *map(str, arguments)], check=True)


def preflight(editor):
    free = shutil.disk_usage(ROOT).free / (1024 ** 3)
    print(f"Selected Unity: {VERSION}")
    print(f"Editor executable present: {editor.is_file()}")
    print(f"Project-volume free space: {free:.1f} GiB")
    print(f"Git LFS available: {shutil.which('git-lfs') is not None}")
    for command in (["xcodebuild", "-version"], ["security", "find-identity", "-v", "-p", "codesigning"]):
        if shutil.which(command[0]):
            subprocess.run(command, check=False)
    return editor.is_file() and free >= 10


def require_editor(editor):
    if not editor.is_file():
        raise RuntimeError(f"Unity is missing: {editor}. Install/activate {VERSION} via Unity Hub first.")
    info_path = editor.parents[1] / "Info.plist"
    if info_path.exists():
        with info_path.open("rb") as stream:
            info = plistlib.load(stream)
        versions = [str(info.get(key, "")) for key in ("CFBundleShortVersionString", "CFBundleVersion")]
        if not any(VERSION in value for value in versions):
            raise RuntimeError(f"Editor does not match pinned version {VERSION}: {versions}")
    if shutil.disk_usage(ROOT).free < 10 * 1024 ** 3:
        raise RuntimeError("Keep at least 10 GiB free on the project volume after installing Unity before importing/building.")


def initialise(editor, template):
    targets = [ROOT / name for name in ("Assets", "Packages", "ProjectSettings")]
    if any(path.exists() for path in targets):
        raise RuntimeError("Project directories already exist. Refusing to overwrite them; use scene/export-ios after reviewing the project.")
    def is_urp_template(path):
        try:
            with tarfile.open(path) as archive:
                metadata = json.load(archive.extractfile("package/package.json"))
            return metadata.get("name") == "com.unity.template.urp-blank"
        except (OSError, tarfile.TarError, KeyError, ValueError, TypeError):
            return False

    if template is None:
        template_dir = editor.parents[1] / "Resources/PackageManager/ProjectTemplates"
        candidates = [path for path in sorted(template_dir.glob("*.tgz")) if is_urp_template(path)]
        if len(candidates) != 1:
            raise RuntimeError("Could not identify one URP blank template. Supply --template with the official URP template downloaded by Unity Hub.")
        template = candidates[0]
    if not template.is_file() or not is_urp_template(template):
        raise RuntimeError("--template must contain the official com.unity.template.urp-blank package.")
    print(f"Using URP template: {template}", flush=True)

    work = ROOT / ".bootstrap-work"
    project = work / "project"
    if project.exists():
        raise RuntimeError("A previous .bootstrap-work/project exists. Inspect its logs before retrying; no existing files were removed.")
    work.mkdir(exist_ok=True)
    logs = ROOT / "Logs"
    logs.mkdir(exist_ok=True)
    run_unity(editor, "-createProject", project, "-cloneFromTemplate", template,
              "-logFile", logs / "create-project.log")
    version_file = project / "ProjectSettings/ProjectVersion.txt"
    if not version_file.is_file() or f"m_EditorVersion: {VERSION}" not in version_file.read_text():
        raise RuntimeError("Generated project version was not verified; inspect Logs/create-project.log.")
    for name in ("Assets", "Packages", "ProjectSettings"):
        if not (project / name).is_dir():
            raise RuntimeError(f"Unity did not generate {name}; inspect the creation log.")
    for name in ("Assets", "Packages", "ProjectSettings"):
        shutil.move(str(project / name), str(ROOT / name))
    destination = ROOT / "Assets/CityRace/Editor"
    destination.mkdir(parents=True)
    shutil.copy2(ROOT / "tools/unity/CityRaceBootstrap.cs", destination / "CityRaceBootstrap.cs")
    print("Project created from Unity template; now importing and generating the smoke scene.")
    execute(editor, "scene")


def execute(editor, action):
    version_file = ROOT / "ProjectSettings/ProjectVersion.txt"
    if not version_file.is_file() or f"m_EditorVersion: {VERSION}" not in version_file.read_text():
        raise RuntimeError("No matching initialised project. Run init first.")
    methods = {
        "scene": ("CityRace.Editor.CityRaceBootstrap.CreateSmokeScene", None),
        "potholes": ("CityRace.Editor.PotholeBootstrap.AddToPractice", None),
        "practice": ("CityRace.Editor.PracticeBootstrap.CreateScene", None),
        "riding": ("CityRace.Editor.RidingBootstrap.CreateScene", None),
        "export-ios": ("CityRace.Editor.CityRaceBootstrap.ExportIos", "iOS"),
        "build-web": ("CityRace.Editor.CityRaceBootstrap.BuildWeb", "WebGL"),
    }
    method, target = methods[action]
    logs = ROOT / "Logs"
    logs.mkdir(exist_ok=True)
    args = ["-projectPath", ROOT, "-logFile", logs / f"{action}.log"]
    if target:
        args += ["-buildTarget", target]
    run_unity(editor, *args, "-executeMethod", method)
    if action == "scene" and not (ROOT / "Assets/CityRace/Content/Scenes/Smoke.unity").is_file():
        raise RuntimeError("Unity exited without generating Smoke.unity; inspect the log.")
    if action == "riding" and not (ROOT / "Assets/CityRace/Content/Scenes/Riding.unity").is_file():
        raise RuntimeError("Unity exited without generating Riding.unity; inspect the log.")
    if not (ROOT / "Packages/packages-lock.json").is_file():
        raise RuntimeError("Package resolution has not produced a lock file; inspect the log.")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("action", choices=["preflight", "init", "scene", "riding", "practice", "potholes", "export-ios", "build-web"])
    parser.add_argument("--editor", type=Path, default=DEFAULT_EDITOR)
    parser.add_argument("--template", type=Path, help="Official URP blank .tgz; optional when found in the editor")
    args = parser.parse_args()
    if args.action == "preflight":
        return 0 if preflight(args.editor) else 1
    try:
        require_editor(args.editor)
        if args.action == "init":
            initialise(args.editor, args.template)
        else:
            execute(args.editor, args.action)
        return 0
    except (RuntimeError, OSError, subprocess.CalledProcessError) as error:
        print(f"BLOCKED: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
