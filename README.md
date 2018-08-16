ShadowsocksR for Windows
=======================

[![Build Status]][Appveyor]

#### 声明
1、绑定域名：
```shadowsocks-csharp/Model/Configuration.cs``` 函数 ```GetDefaultUrl()```

2、Api json声明
```php
        $res['ret'] = ...
        $res['msg'] = ...
        $res['data']['email'] = ...
        $res['data']['class'] = ...
        $res['data']['class_expire'] = ...
        $res['data']['unusedTraffic'] = ...
        $res['data']['TodayusedTraffic'] = ...
        $res['data']['ssr_url_all'] = ...
        $res['data']['ssr_url_all_mu'] = ...
```
```shell
{
  'ret' : '..',
  'msg' : '..',
  'data' : {
    '' : '',
    ...
  }
}
```


#### Download

<a href="https://github.com/00a7a00/shadowsocksr-csharp/releases" >Download</a>

#### Develop

1、下载 Visual Studio

2、```git clone https://github.com/00a7a00/shadowsocksr-csharp```

3、......


#### 发布

1、打开 Visual Studio

2、「批生成」

3、详细可参考 <a href="https://github.com/00a7a00/shadowsocksr-csharp/releases" >Download 页面</a>

#### License

GPLv3

Copyright © BreakWa11 2017. Fork from Shadowsocks by clowwindy

[Appveyor]:       https://ci.appveyor.com/project/breakwa11/shadowsocksr-csharp
[Build Status]:   https://ci.appveyor.com/api/projects/status/itcxnad1y95gf2x5/branch/master?svg=true
[latest release]: https://github.com/shadowsocksr/shadowsocksr-csharp/releases
