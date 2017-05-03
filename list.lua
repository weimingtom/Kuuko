-- from https://github.com/weimingtom/MoonSharpMod/blob/master/moonsharpsl4/MainPage.xaml.cs

local function pad(str, len)
	str = str .. ' ' .. string.rep('.', len);
	str = string.sub(str, 1, len);
	return str;
end

function list(lib)
	if (lib == nil) then lib = _G; end
	if (type(lib) ~= 'table') then
		print('A table was expected to list command.');
		return
	end
	for k, v in pairs(lib) do
		print(pad(type(v), 12) .. ' ' .. k)
	end
end
