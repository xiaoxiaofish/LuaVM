# LuaVM

一个基于C#的垃圾Lua虚拟机。

这是一个完全使用C#进行开发的Lua虚拟机项目，包括编译器，虚拟机和Lua标准库三部分。

当然现在项目还没有完成，暂时只完成了编译器和虚拟机，以及很多很多的bug。

鉴于我并不会写单元测试，所以项目中还存在着大量的bug，我也不知道哪里有bug，欢迎你向我提出意见，谢谢。

目前的调试方式是在bin\Define 目录下有个code.text的文档，虚拟机会加载这个文档并当做lua源代码文件解释执行。

但是现在我开始了另一个项目，所以更便捷的加载方式和修复bug等等工作需要等到之后再来解决。

这个项目的最终目的是想要在unity上实现一个可以和C#互相调用的Lua虚拟机，并以跨平台的jit方式进行执行。并且在unity平台上以插件的形式提供IDE功能，如果你对这个项目感兴趣的话，欢迎加入！
