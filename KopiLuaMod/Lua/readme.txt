http://gerich-home.github.io/kopilua/

kopilua by gerich-home
C# port of Lua 5.1.4
Kopi Lua is a C# port of the Lua v.5.1.4 virtual machine, parser, libraries and command-line utilities. The latest version of Kopi Lua can be downloaded from the Kopi Lua web page: http://www.ppl-pilot.com/KopiLua

Install

Kopi Lua is implemented in C# and has been tested on Microsoft Visual C# 2008 Express Edition. In order for Kopi Lua to successfully compile the following conditional compilation symbols must be defined in the project settings: LUA_CORE;_WIN32;LUA_COMPAT_VARARG;LUA_COMPAT_MOD;LUA_COMPAT_GFIND;CATCH_EXCEPTIONS

License

MIT license

Authors

Mark Feldman (lua@ppl-pilot.com) 
gerich-home (gerich.home@gmail.com) 

Contact

Mark Feldman (lua@ppl-pilot.com) 
gerich-home (gerich.home@gmail.com) 
Download

You can download this project in either zip or tar formats.

You can also clone the project with Git by running:

$ git clone git://github.com/gerich-home/kopilua


-----------------------

mod

1. delete using lu_mem = System.UInt32;
2. lua_TValue -> TValue
3. delete partial class
4. move class/emum/struct out
5. uint->int
6. using



todo:
remove ref

-----------------------

LuaAuxLib
LuaLoadLib
LuaObject
LuaParser
LuaPrint
LuaStrLib
LuaTableLib
LuaVM
