@echo off

set PATH=%PATH%;C:\Program Files (x86)\Java\jre7\bin\

%~dp0..\Python\App\python.exe "%~dp0playgame.py" ^
 --engine_seed 42 ^
 --player_seed 42 ^
 --end_wait=0.25 ^
 --verbose ^
 -E -e ^
 --log_dir game_logs ^
 --turns 100 ^
 --scenario ^
 --food none ^
 --map_file "%~dp0maps\example\tutorial1.map" %* ^
 "%~dp0..\your bot\bin\Debug\MyBot.exe" ^
 "%~dp0..\Python\App\python.exe ""%~dp0sample_bots\python\HunterBot.py"""