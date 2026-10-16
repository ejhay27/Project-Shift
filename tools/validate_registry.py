#!/usr/bin/env python3
"""Validate registry/feature_registry.json integrity.

Checks: required fields, unique IDs, valid status/authority, milestone 0-10,
dependency references exist, no self-dependency, code_refs paths exist.
Exit 0 on success, 1 on any error.
"""
import json
import os
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
REGISTRY = os.path.join(ROOT, "registry", "feature_registry.json")
REQUIRED = ["id", "title", "category", "spec", "status", "milestone",
            "depends_on", "authority", "save", "acceptance"]


def main() -> int:
    errors: list[str] = []
    with open(REGISTRY, encoding="utf-8") as f:
        reg = json.load(f)

    entries = reg.get("entries", [])
    ids = [e.get("id") for e in entries]
    if len(ids) != len(set(ids)):
        errors.append("duplicate entry ids found")
    idset = set(ids)

    status_values = set(reg.get("status_values", []))
    authority_values = set(reg.get("authority_values", []))

    for e in entries:
        eid = e.get("id", "<missing>")
        for field in REQUIRED:
            if field not in e:
                errors.append(f"{eid}: missing field '{field}'")
        if e.get("status") not in status_values:
            errors.append(f"{eid}: invalid status '{e.get('status')}'")
        if e.get("authority") not in authority_values:
            errors.append(f"{eid}: invalid authority '{e.get('authority')}'")
        ms = e.get("milestone")
        if not isinstance(ms, int) or not 0 <= ms <= 10:
            errors.append(f"{eid}: milestone must be int 0..10, got {ms!r}")
        for dep in e.get("depends_on", []):
            if dep == eid:
                errors.append(f"{eid}: depends on itself")
            elif dep not in idset:
                errors.append(f"{eid}: unknown dependency '{dep}'")
        for ref in e.get("code_refs", []):
            if not os.path.exists(os.path.join(ROOT, ref)):
                errors.append(f"{eid}: code_ref missing: {ref}")
        # Honesty rule: 'tested' requires evidence note.
        if e.get("status") == "tested" and "evidence" not in (e.get("notes") or "").lower():
            errors.append(f"{eid}: status 'tested' requires evidence in notes")

    # Summary
    from collections import Counter
    counts = Counter(e.get("status") for e in entries)
    print(f"registry entries: {len(entries)}")
    for k in sorted(counts):
        print(f"  {k}: {counts[k]}")
    print(f"milestones covered: {sorted(set(e.get('milestone') for e in entries))}")

    if errors:
        print(f"\nERRORS ({len(errors)}):")
        for err in errors:
            print(f"  - {err}")
        return 1
    print("registry OK")
    return 0


if __name__ == "__main__":
    sys.exit(main())
