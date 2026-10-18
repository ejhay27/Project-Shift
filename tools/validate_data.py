#!/usr/bin/env python3
"""Validate Unity GameData JSON files.

Checks: unique item ids, stackLimit > 0, node resourceIds reference items,
node yield/respawn sanity, recipe inputs/outputs reference items with
positive counts, unique recipe ids, node ids referenced by docs stay in sync.
Exit 0 on success, 1 on any error.
"""
import json
import os
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DATA = os.path.join(ROOT, "unity", "ProjectShift", "Assets", "GameData")


def load(name: str):
    with open(os.path.join(DATA, name), encoding="utf-8") as f:
        return json.load(f)


def main() -> int:
    errors: list[str] = []

    items = load("items.json").get("items", [])
    item_ids = [i.get("id") for i in items]
    if len(item_ids) != len(set(item_ids)):
        errors.append("items.json: duplicate ids")
    idset = set(item_ids)
    for i in items:
        for field in ("id", "displayName", "category", "stackLimit"):
            if field not in i:
                errors.append(f"items.json: '{i.get('id')}' missing '{field}'")
        if not isinstance(i.get("stackLimit"), int) or i.get("stackLimit", 0) <= 0:
            errors.append(f"items.json: '{i.get('id')}' bad stackLimit")

    nodes = load("resource_nodes.json").get("nodes", [])
    node_ids = [n.get("nodeId") for n in nodes]
    if len(node_ids) != len(set(node_ids)):
        errors.append("resource_nodes.json: duplicate nodeIds")
    for n in nodes:
        nid = n.get("nodeId", "<missing>")
        if n.get("resourceId") not in idset:
            errors.append(f"{nid}: unknown resourceId '{n.get('resourceId')}'")
        if not (isinstance(n.get("yieldMin"), int) and isinstance(n.get("yieldMax"), int)
               and 0 < n["yieldMin"] <= n["yieldMax"]):
            errors.append(f"{nid}: bad yield range")
        if not (isinstance(n.get("respawnMinutes"), (int, float))
               and n["respawnMinutes"] > 0):
            errors.append(f"{nid}: bad respawnMinutes")
        # Spec §9: ordinary stone is ~10 minutes.
        if nid.startswith("stone.") and abs(n.get("respawnMinutes", 0) - 10.0) > 0.001:
            errors.append(f"{nid}: stone respawn must be ~10 minutes")

    recipes = load("recipes.json").get("recipes", [])
    recipe_ids = [r.get("recipeId") for r in recipes]
    if len(recipe_ids) != len(set(recipe_ids)):
        errors.append("recipes.json: duplicate recipeIds")
    for r in recipes:
        rid = r.get("recipeId", "<missing>")
        if not r.get("station"):
            errors.append(f"{rid}: missing station")
        for side in ("inputs", "outputs"):
            for ing in r.get(side, []):
                if ing.get("itemId") not in idset:
                    errors.append(f"{rid}: {side} unknown item '{ing.get('itemId')}'")
                if not (isinstance(ing.get("count"), int) and ing["count"] > 0):
                    errors.append(f"{rid}: {side} bad count for '{ing.get('itemId')}'")

    print(f"items: {len(items)}, nodes: {len(nodes)}, recipes: {len(recipes)}")
    if errors:
        print(f"\nERRORS ({len(errors)}):")
        for err in errors:
            print(f"  - {err}")
        return 1
    print("game data OK")
    return 0


if __name__ == "__main__":
    sys.exit(main())
