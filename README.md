## CFET基本介绍
* [GettingStartedWithCFET](./Doc/design/wiki/GettingStartedWithCFET2.md)

* [如何访问CFET资源](./Doc//design/wiki/AccessingResources/AccessingResources.md)

## 如何运行？

需要搭载在CFET2APP中运行，由于CFET core的项目毕竟多，可以在CFET2Core的解决方案中引用CFET2APP，具体步骤如下：

1. git clone CFET core
2. 在本地文件CFETcore的路径下，新建CFET2APP子文件夹，在子文件夹中clone CFET2APP
3. 打开CFETcore sln，添加CFET2APP项目，设置CFET2APP为启动项目
4. 卸载CFET2APP的nuget包中CFET2Core的引用，改为本地引用CFET2Core，点击运行即可

## 修改之后如何更新到git？
由于core的repo里面最好不要有CFET2APP，所以上传之前先把CFET2APP从解决方案中移除，再在文件夹中直接删除CFET2APP，在add/commit/push

CFET2Core的改动非常少，且大部分时候也是用来看的，不影响thing的单独上传，这么做虽然core的迭代麻烦了很多，但实际方便了CFET2APP和Thing的开发以及管理

## publish
支持多平台需要发布项目

在sln文件或者csproj文件下执行即可生成发布文件夹，里面包含了所需的依赖

```
dotnet publish -f net6.0 -r linux-arm64 --configuration Release --no-self-contained
dotnet publish -f net6.0 -r linux-x64 --configuration Release --no-self-contained
```

其中需要注意的是System.IO.Ports不同框架对应不同dll，引用该类的一定记的publish的时候指定目标runtime和框架

## nuget包上传

更新了项目之后需要更新对应的nuget包，修改包的版本后双击AutoNuget.bat即可

nuget pack
for /f "delims=" %%a in ('dir /b/a-d/oN *.nupkg') do set n=%%a
nuget push %n% jushen -Source http://222.20.94.134:14999/api/v2/package
pause

