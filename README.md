# AdminTool
为公司GM开发的工具，可以文本配置gm指令，规则

- #代表单行注释
- 支持的配置：uids，host，port，cmd
- 每个cmd都要去含有：group，名字，cmd-tag，(uid),[args],其中uid是默认自动添加的，args可有可无，可以有多个，以逗号分隔,args支持下拉菜单，关键字为:options，例如：options=金币:1|钻石:2|万能图纸:3|高级万能图纸:4|原油:5

- Demo：cmd.cfg
<pre><code>
#$代表变量，@代表需要base64编码的变量
#cmd要求：group,名字，cmd，$uid, $args,uid如果没写则自动添加
uids = u72620561171318476,u72620561171318477
host = 192.168.2.169, 192.168.4.105
port = 2020
#todo:ComboBox,别名支持,对于金钱等形式
#cmd格式:group,showname,cmd,args,无需填写uid,默认会自动添加
cmd = 玩家, 添加货币, 		addmoney,$type=1[options=金币:1|钻石:2|万能图纸:3|高级万能图纸:4|原油:5],$count=1000
cmd = 玩家, 添加物品, 		additem, $itemid, $num
cmd = 玩家, 删除物品,		delitem, #itemid, $num
cmd = 玩家, 添加玩家经验,	addexp, $exp
cmd = 玩家, 设置玩家等级, 	setlevel,$level
cmd = 玩家, 添加vip经验,	addvipexp,$exp
cmd = 玩家, 设置vip等级,	setviplevel,$level
cmd = 玩家, 跳过新手引导,	skipguide
cmd = 玩家, 重置账号,		resetaccount

cmd = 战舰, 添加战舰, 		addtank, $shipid,$num
cmd = 战舰, 删除战舰,		delship, $shipid
cmd = 战舰,	设置战舰等级,	setshiplv,$shipid,$level

cmd = 图纸,	添加图纸,		addpaper,$paperid,$num
cmd = 图纸,	添加图纸碎片,	addpaperpiece, $paperid,$num

cmd = 配件, 添加配件, 		addparts,$partsid,$num
cmd = 配件, 删除配件,		delparts,$partsid
cmd = 配件, 设置配件等级,	setpartslv,$partsid, $lv

cmd = 副本, 重置副本次数,	resetstagecount,$stageid
cmd = 副本, 设置副本胜利,	winstage,$stageid
cmd = 副本, 设置副本失败, 	losestage,$stageid
cmd = 副本, 重置副本次数, 	resetstagecount,$stageid
</code></pre> 