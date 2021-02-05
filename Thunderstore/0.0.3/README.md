This plugin allows you to edit how the teleporter item drop behaves.

The following options are available:
 - Drop Uniformity: The teleporter boss wil only drop one item type instead of multiple (ie. some greens and some yellows).
 - Select what tiers of items can be dropped from the teleporter.
 - Set a weighted chance for each tier to be chose to spawn.
 - Set a multiplier for the amount of items of that specific tier that will be spawned.

Changelog:
 - 0.0.1: Initial Release
 - 0.0.2: Changed target assembly version from netcore 2.0 to netstandard 2.0.
 - 0.0.3: Bugfixes: 
	- Fixed an issue where no boss drops could cause no items to be spawned. 
	- Fixed an issue that caused the right tier to not be selected fairly (according to the weight).