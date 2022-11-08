### Project Information
Project contains many of typical rpg functionalities that are listed in features list below. 
It is using mouse-to-move controls. Project was done with usage of namespaces, assembly definitions, scriptable objects, external asset packs, prefab variants. 
It has custom dialog node editor. 
As of now some functionalities contain bugs.
- [Project image album](https://imgur.com/a/JIdUpyz)
- [Production build as zip](https://drive.google.com/file/d/1pnZZAJTm8gMF09cJHGFi9TkXLNdkZ5vV/view?usp=sharing)

### Input Information
Input | Action
--- | ---
LMB |  move / attack / interact
1-6 (alpha-numeric) | quickslot abilities
I |  inventory & equipment menu
T | trait menu
Q | quest menu
Esc | pause menu


### Features
+ Dialogues node editor (using scriptable objects)
	+ Possibility of adding complex conditions using AND and OR statements (although reliant on string references)
+ Inventory & Equipment
	+ Stackable and one-off items
+ Enemy drops
+ Quickslot Abilities
+ Currency & shops
	+ Ability to limit equiping based on level requirements
+ Enemy and Player Progression
+ Traits
	+ Ability to add eqipment trait conditions (although using string references)
+ Basic Enemy AI (based on distance)
+ On hit vfx & sfx
+ Basic combat animations
+ Changing cursor icon depending on context
+ Mouse targeting movement and combat
+ Respawn with minimal health instead of reloading scene 
	+  Restore some of enemies health on player death
+ Main menu, Pause menu
+ Saving and loading (unlimited number of saves)
+ Health & mana (mana used for fireball, fireball is unobtainable as of now)
+ Projectile weapons (bow and fireball, no way to obtain for player as of now, bow is weapon for one of enemies)
---
+ Multiple choice dialogs with triggers for events (currently not working in non-editor due to reliance on editor funcitonality)
+ Quests (currently not working in non-editor due to reliance on editor funcitonality)

### Limitations
+ Heavy reliance on mouse input
+ No information about input in game, no ui to toggle menus
+ No way to delete saves from production build
+ Various sorts of bugs when going to 2nd scene (currently disabled)
+ Some dialogue lines are skipped
+ Quest and dialogue system reliance on string references
+ Ability to interact with npcs from unlimited distance
+ If potions (stackable items) are equipped to action bar, buying new potions does not add them to action bar, but to new inventory stack
+ Only debug menu for health, mana, enemy health, experience, level
+ No background audio
+ Menus are always displayed in the same order

### Courses links
- [Core RPG](https://www.gamedev.tv/p/unity-rpg/?coupon_code=AUTUMN)
- [Inventories](https://www.gamedev.tv/p/inventory)
- [Dialogs and Quests](https://www.gamedev.tv/p/rpg-dialogue-quests-intermediate-c-game-coding-course/?coupon_code=AUTUMN)
- [Shops and Abilities](https://www.gamedev.tv/p/rpg-shops-skills-abilities-game-development/?coupon_code=AUTUMN)