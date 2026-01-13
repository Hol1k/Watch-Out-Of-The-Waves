# Watch-Out-Of-The-Waves

**Session-based tower defense prototype with raft building, resource loot, and wave pressure.**

This project explores a compact gameplay loop inspired by Raft-style tile construction and survival looting, combined with tower-defense pacing:
**expand the raft → build defenses → survive waves**.

---

## Current Prototype Features

- Raft tile building (grid/tiles) with blueprint placement flow
- Inventory + building costs (spend resources to build)
- Loot system: lootable world items + fixes for double-loot edge cases
- Destructible objects / loot containers (wooden loot box)
- Placeable tower (cannon) that targets and shoots nearby enemies
- Tower progression: upgrading + leveling logic
- Enemy baseline: movement to target, target selection, and attack behavior
- Wave-based enemy manager/spawner
- Enemy loot drops from their inventory
- Player controller: movement + camera follow + adjustable camera range
- Player actions: jump + melee/attack input iterations (click → hold for auto-attack)
- Shark enemy content: custom shark model, animation, and behavior setup

---

## Gameplay Loop

1. Collect resources (dropped loot + breakable loot containers)
2. Expand the raft by placing tiles and structures (build mode)
3. Spend inventory items to construct towers / tiles and evolve defenses
4. Survive enemy waves as sharks and other enemies pressure the raft

---

## Implemented Systems

### Raft Building
- Tile-based raft foundation (“planes/tiles”), including a dedicated core tile concept used as the starting foundation
- Build-mode flow driven by a player state machine (Default / BuildMode)
- Blueprint activation/selection improvements (prevent accidental builds when a tile isn’t selected)
- Ability to move placed buildings via the player interaction layer

### Buildings, Damage, and Destruction
- IDamageable + health logic applied to building pieces
- On-destruction behavior that can clean up or affect nearby blueprint/tile state (destruction propagation)

### Inventory, Loot, and Costs
- Inventory foundation + initial building cost rules
- Loot exists as world objects that can be collected via a looting component
- Fixes for repeated/double loot interactions
- HUD inventory display iteration
- Loot boxes that drop resources when destroyed

### Towers (Cannon Defense)
- Placeable tower prototype (cannon) with target acquisition + projectile logic
- Tower progression: upgrade flow + leveling logic
- Simple cannon model added to support the tower loop visually

### Enemies & Waves
- Enemies move toward a target, with an ocean collider baseline for world boundaries/interaction
- Target selection and attack targeting logic (choose target → attack target)
- Enemies can carry inventory and drop loot on death
- Wave-based enemy manager orchestrates spawns/pacing
- Shark content: model import + animation + behavior implementation

### Abilities (Prototype Layer)
- Abstract ScriptableObject-based ability foundation
- Example abilities implemented:
  - Rapid Fire
  - High-Power Volley

---

## Tech Stack

- Unity (URP)
- C#
- Unity Input System

---

## Technical Documentation

- Design & development wiki (Notion): <NOTION_PUBLIC_LINK_HERE>
- Task board (YouGile): <YOUGILE_BOARD_LINK_HERE> (Kanban: ToDo / InProg / Done)

---

## Project Status

This project is **not actively developed at the moment**.

It represents a completed gameplay prototype focused on raft building, inventory systems, and tower-defense mechanics.
