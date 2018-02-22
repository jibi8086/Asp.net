// JavaScript source code
// created by deepu

function emoji() {
    var a = [[':happy;', '<img src="//cdn.shopify.com/s/files/1/1061/1924/products/Slightly_Smiling_Emoji_Icon_34f238ed-d557-4161-b966-779d8f37b1ac.png?v=1485577096"  style="height:20px;width:20px;padding:0px;margin:0px;"/>'],
            [':sick;', "<img src='https://media1.tenor.com/images/019d4b615d97b7fe2cd2692465485d04/tenor.gif?itemid=7387814' style='height:20px;width:20px;padding:0px;margin:0px;' />"]];
    a.forEach(function (item) {
        $('body').each(function () {
            var text = $(this).html();
            $(this).html(text.replace(item[0], item[1]));
        });
    });
}