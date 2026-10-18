#!/usr/bin/env python3
"""Static smoke checks for C# sources (no compiler in sandbox).

Checks:
  1. ProjectShift.Core files reference no UnityEngine/UnityEditor APIs.
  2. Brace balance per file (rough syntax sanity, comments/strings stripped).
  3. Public type name matches file name (one primary type per file).
  4. No 'NotImplementedException' stubs (no fake-complete systems).
This does NOT replace a real compile; Unity/dotnet compile happens on dev
machines/CI (TECH-002/TECH-003). Exit 0 on success, 1 on any error.
"""
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SCRIPTS = os.path.join(ROOT, "unity", "ProjectShift", "Assets", "Scripts")
CORE = os.path.join(SCRIPTS, "Core")

FORBIDDEN_IN_CORE = ["UnityEngine", "UnityEditor", "Unity.", "MonoBehaviour",
                     "ScriptableObject", "InputSystem"]


def strip_comments_strings(src: str) -> str:
    src = re.sub(r'@"(?:[^"]|"")*"', '""', src)          # verbatim strings
    src = re.sub(r'"(?:\\.|[^"\\])*"', '""', src)         # strings
    src = re.sub(r"'(?:\\.|[^'\\])*'", "''", src)         # chars
    src = re.sub(r"//.*", "", src)                        # line comments
    src = re.sub(r"/\*.*?\*/", "", src, flags=re.S)       # block comments
    return src


def main() -> int:
    errors: list[str] = []
    cs_files: list[str] = []
    for dirpath, _, files in os.walk(SCRIPTS):
        for name in sorted(files):
            if name.endswith(".cs"):
                cs_files.append(os.path.join(dirpath, name))

    if not cs_files:
        print("ERROR: no C# files found")
        return 1

    for path in cs_files:
        rel = os.path.relpath(path, ROOT)
        with open(path, encoding="utf-8") as f:
            src = f.read()

        if path.startswith(CORE):
            for token in FORBIDDEN_IN_CORE:
                if token in src:
                    errors.append(f"{rel}: Core must not reference '{token}'")

        if "NotImplementedException" in src or "throw new NotImplemented" in src:
            errors.append(f"{rel}: contains NotImplemented stub")

        clean = strip_comments_strings(src)
        if clean.count("{") != clean.count("}"):
            errors.append(f"{rel}: unbalanced braces "
                          f"({clean.count('{')} open vs {clean.count('}')} close)")

        expected = os.path.splitext(os.path.basename(path))[0]
        # Find public type declarations; at least one must match the file name
        # (asmdef-adjacent helper files with no public type are flagged).
        decls = re.findall(
            r"public\s+(?:sealed\s+|static\s+|abstract\s+|readonly\s+)*"
            r"(?:class|struct|enum|interface)\s+(\w+)", clean)
        if expected not in decls:
            errors.append(f"{rel}: no public type '{expected}' "
                          f"(found: {', '.join(decls) or 'none'})")

    print(f"C# files checked: {len(cs_files)}")
    if errors:
        print(f"\nERRORS ({len(errors)}):")
        for err in errors:
            print(f"  - {err}")
        return 1
    print("C# static checks OK (compile still required in Unity/dotnet)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
