build:
	xbuild source/Liquid.sln

clean:
	rm -rf source/bin source/obj

example: build
	mono source/bin/Debug/Liquid.exe "__Shor(15,true)"

.PHONY: build clean example

.DEFAULT_GOAL := build
