build:
	xbuild source/Liquid.sln

clean:
	find . -name Liquid.log -type f -delete
	rm -rf source/bin source/obj

example:
	mono bin/Liquid.exe "__Shor(15,true)"

.PHONY: build clean example

.DEFAULT_GOAL := build
