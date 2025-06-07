@echo off
REM Tạo cấu trúc thư mục cho Animal Revolt Game

REM Characters
md "Assets\AnimalRevolt\Characters\Models"
md "Assets\AnimalRevolt\Characters\Animations"
md "Assets\AnimalRevolt\Characters\Prefabs"
md "Assets\AnimalRevolt\Characters\ScriptableObjects"

REM Combat
md "Assets\AnimalRevolt\Combat\Skills"
md "Assets\AnimalRevolt\Combat\Weapons"
md "Assets\AnimalRevolt\Combat\Effects"

REM Ragdoll
md "Assets\AnimalRevolt\Ragdoll\Components"
md "Assets\AnimalRevolt\Ragdoll\Configs"
md "Assets\AnimalRevolt\Ragdoll\Materials"

REM GameModes
md "Assets\AnimalRevolt\GameModes\Battle"
md "Assets\AnimalRevolt\GameModes\Survival"
md "Assets\AnimalRevolt\GameModes\Tournament"

REM Arena
md "Assets\AnimalRevolt\Arena\Maps"
md "Assets\AnimalRevolt\Arena\Props"
md "Assets\AnimalRevolt\Arena\Materials"

REM UI
md "Assets\AnimalRevolt\UI\HUD"
md "Assets\AnimalRevolt\UI\Menus"
md "Assets\AnimalRevolt\UI\Battle"
md "Assets\AnimalRevolt\UI\Prefabs"

REM Audio
md "Assets\AnimalRevolt\Audio\Music"
md "Assets\AnimalRevolt\Audio\SFX"
md "Assets\AnimalRevolt\Audio\Voice"

REM Scripts
md "Assets\AnimalRevolt\Scripts\Core"
md "Assets\AnimalRevolt\Scripts\Characters"
md "Assets\AnimalRevolt\Scripts\Combat"
md "Assets\AnimalRevolt\Scripts\Ragdoll"
md "Assets\AnimalRevolt\Scripts\GameModes"
md "Assets\AnimalRevolt\Scripts\UI"
md "Assets\AnimalRevolt\Scripts\Managers"
md "Assets\AnimalRevolt\Scripts\Utilities"

REM Settings
md "Assets\AnimalRevolt\Settings\Input"
md "Assets\AnimalRevolt\Settings\Graphics"
md "Assets\AnimalRevolt\Settings\Audio"

echo Tạo cấu trúc thư mục thành công!
pause