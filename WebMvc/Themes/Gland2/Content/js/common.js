function parseTemplate(e,n){var r,t=n;for(r in e)e.hasOwnProperty(r)&&(t=t.replace(new RegExp("{{"+r+"}}","g"),e[r]));return t}function checkBrowserEnableCookie(){var e=!!navigator.cookieEnabled;return"undefined"!=typeof navigator.cookieEnabled||e||(document.cookie="testcookie",e=-1!=document.cookie.indexOf("testcookie")),!!e}function hura_create_cookie(e,n,r){if(r){var t=new Date;t.setTime(t.getTime()+24*r*60*60*1e3);var o="; expires="+t.toGMTString()}else var o="";document.cookie=e+"="+Base64.encode(n)+o+"; path=/;"}function hura_read_cookie(e){for(var n=e+"=",r=document.cookie.split(";"),t=0;t<r.length;t++){for(var o=r[t];" "==o.charAt(0);)o=o.substring(1,o.length);if(0==o.indexOf(n))return Base64.decode(o.substring(n.length,o.length))}return null}function format_number(e){function n(e){for(e+="";e.indexOf(".")>0;)e=e.replace(".","");var n=parseFloat(e);return isNaN(n)?0:n}if("0"==e||0==e)return"";e=n(e)+"",e=e.replace(/\s/g,"");var r=e.indexOf(",")>0?e.substr(e.indexOf(",")):"";""!=r&&(e=e.substr(0,e.indexOf(","))),e=(e+"").replace(/\./g,"");for(var t=e.substr(0,e.length%3),o=e.replace(t,""),a=o.length/3,c="",i="",u=0;a>u;u++)i=o.substr(3*u,3),c+=i,u!=a-1&&(c+=".");return t.length>0?""!=c?t+"."+c+r:t+r:""!=c?c+r:""}function hura_erase_cookie(e){hura_create_cookie(e,"",-1)}function countShoppingCart(e){var n=document.getElementById("count_shopping_cart_store"),r=document.getElementById("total_shopping_cart_store");if(null==hura_read_cookie(e))hura_create_cookie(e,"-",30),"underfined"!=n&&null!=n&&(n.innerHTML=0);else{var t=hura_read_cookie(e),o=t.split(","),a=o.length;if("underfined"!=n&&null!=n&&(n.innerHTML=a-1),"underfined"!=r&&null!=r&&$("#total_shopping_cart_store").length>0){for(var c,i,u,l,d,h,_=0,s=0;s<o.length;s++)c=o[s],c.length>1&&(i=c.split("-"),u=i[0],l=i[1],d=parseFloat(i[2]),h=parseFloat(i[3]),_+=h*d);r.innerHTML=writeStringToPrice(_+"")}}}function emptyShoppingCart(e){hura_create_cookie(e,"-",30)}function addItemToCart(e,n,r,t){null==hura_read_cookie("shopping_cart_store")&&hura_create_cookie("shopping_cart_store",",",30);var o=hura_read_cookie("shopping_cart_store");if(-1==o.search(","+e+"-"+n+"-")){var a=o+","+e+"-"+n+"-"+r+"-"+t;hura_create_cookie("shopping_cart_store",a,30)}"pro"==e&&ProductAddon.checkSelect(n)}function addToShoppingCart(e,n,r,t){addItemToCart(e,n,r,t),window.location=SHOPPING_CART_URL}function addToShoppingCartStop(e,n,r,t,o){addItemToCart(e,n,r,t);var a=document.getElementById(o);"undefined"!=typeof a&&null!=a&&$("#"+o).html("Đã thêm vào giỏ hàng")}function deleteShoppingCartItem(e,n){confirm("Bạn muốn xóa bỏ sản phẩm này khỏi giỏ hàng ? ")&&(removeShoppingCartItem(e,n),window.location=SHOPPING_CART_URL)}function removeShoppingCartItem(e,n){var r=hura_read_cookie("shopping_cart_store");if(null!=r){r+=",";var t=new RegExp(","+e+"-"+n+"-(.*?)-(.*?),","i"),o=r.replace(t,","),o=o.substr(0,o.length-1);hura_create_cookie("shopping_cart_store",o,30),ProductAddon.removeProduct(n)}}function writeStringToPrice(e){var n="",r="usd"==default_price_currency?",":".";"usd"==default_price_currency&&(n=-1!=e.lastIndexOf(".")?e.replace(e.substr(0,e.lastIndexOf(".")),""):"",e=e.replace(n,""));for(var t=e.substr(0,e.length%3),o=e.replace(t,""),a=o.length/3,c="",i=0;a>i;i++)group_of_three=o.substr(3*i,3),c+=group_of_three,i!=a-1&&(c+=r);return t.length>0?""!=c?t+r+c+n:t+n:""!=c?c+n:""}function getItemUnitPrice(e,n){for(var r=document.getElementById("sell_price_"+e+"_"+n).innerHTML;r.indexOf(".")>0;)r=r.replace(".","");return r=parseInt(r)}function updatePrice(e,n,r){check_interger(r)||(alert("Vui lòng nhập số > 0"),r=1,$("#quantity_"+e+"_"+n).val(r)),show_cart_total(e,n,r);var t=$("#discount_code").val();if(t.length>0){for(var o=document.getElementById("total_value").innerHTML;o.indexOf(".")>0;)o=o.replace(".","");check_discount("coupon",t,parseInt(o))}}function show_cart_total(e,n,r){var t=getItemUnitPrice(e,n);document.getElementById("price_"+e+"_"+n).innerHTML=writeStringToPrice(t*r+""),reCountTotal()}function check_interger(e){var n=/^\d+$/;return n.test(e)?parseInt(e)>0:!1}function reCountTotal(){for(var e,n,r,t,o,a="",c=document.getElementById("item_update_quantity").value,i=c.split(","),u=0,l=0;l<i.length;l++)if(e=i[l],e.length>0){if(n=document.getElementById("sell_price_"+e.replace("-","_")).innerHTML,"usd"==o)for(;n.indexOf(",")>0;)n=n.replace(",","");else for(;n.indexOf(".")>0;)n=n.replace(".","");r=parseFloat(n),t=parseInt(document.getElementById("quantity_"+e.replace("-","_")).value),a+=","+e+"-"+t+"-"+r,u+=r*t}hura_create_cookie("shopping_cart_store",a,30),document.getElementById("total_value").innerHTML=writeStringToPrice(u+""),document.getElementById("total_cart_value").value=u;var d=document.getElementById("total_shopping_cart_store");"undefined"!=d&&null!=d&&(d.innerHTML=writeStringToPrice(u+""))}function isNumber(e){return!isNaN(parseFloat(e))&&isFinite(e)}function makeUrlAcceptQuery(e){return e.search("?")>0?e:e+"?"}function checkCartForm(){var e=hura_read_cookie("shopping_cart_store");if(null==e)return!1;var n=e.split(","),r=n.length;if(1>=r)return confirm("Giỏ hàng chưa có sản phẩm. Vui lòng chọn sản phẩm vào giỏ hàng")&&(window.location="/"),!1;var t="",o=document.getElementById("buyer_name").value;o.length<4&&(t+="- Bạn chưa nhập tên\n");var a=document.getElementById("buyer_mobile").value;a.length<5&&(t+="- Bạn chưa nhập số di động\n");var c=document.getElementById("buyer_address").value;return c.length<10&&(t+="- Bạn chưa nhập địa chỉ"),""!=t?(alert(t),!1):!0}function add_compare_product(e){var n=document.getElementById("product_compare_list");if("undefined"==n||null==n)return alert("Cần có biến product_compare_list trong template"),!1;""==n.value&&(n.value=",");var r=0;n.value.length>1&&(current_list_id=n.value.split(","),r=current_list_id.length-1);var t=document.getElementById("compare_box_"+e),o=document.getElementById("text_compare_"+e);t.checked?r>6?(t.checked="",alert("Bạn chỉ có thể so sánh tối đa 6 sản phẩm\nDanh sách đã có đủ 6")):(document.getElementById("product_compare_list").value=n.value+e+",","undefined"!=o&&null!=o&&(o.innerHTML="Chờ so sánh",o.style.backgroundColor="#FFCC00")):(document.getElementById("product_compare_list").value=n.value.replace(","+e+",",","),"undefined"!=o&&null!=o&&(o.innerHTML="Chọn so sánh ",o.style.backgroundColor="#FFF"))}function compare_product(){var e=document.getElementById("product_compare_list");if("undefined"==e||null==e)return alert("Cần có biến product_compare_list trong template"),!1;var n=0;return e.value.length>1&&(current_list_id=e.value.split(","),n=current_list_id.length-2),n>1?void(window.location="/so-sanh?list="+e.value):(alert("Bạn cần chọn tối thiểu 2 sản phẩm để so sánh"),!1)}function save_product_view_history(e){if(!check_interger(e))return!1;var n="product_view_history";null==hura_read_cookie(n)&&hura_create_cookie(n,",",30);var r=hura_read_cookie(n);if(-1==r.search(","+e+",")){var t=r+","+e+",";hura_create_cookie(n,t,30)}}function remove_from_history(e){if(confirm("Bạn chắc chắn muốn xóa sản phẩm này ?")){if(!check_interger(e))return!1;var n="product_view_history";null==hura_read_cookie(n)&&hura_create_cookie(n,",",30);var r=hura_read_cookie(n);if(-1!=r.search(","+e+",")){var t=r.replace(","+e+",",",");hura_create_cookie(n,t,30)}window.location=window.location}}function user_like_content(e,n,r){$.post("/ajax/user_like.php",{item_id:e,content_type:n},function(e){"error-not-login"==e?confirm("Để sử dụng chức năng này bạn cần đăng nhập. Click OK để đăng nhập")&&(window.location="/dang-nhap?returnTo="+window.location):"error"==e?alert("Bạn đã thực hiện rồi"):$("#"+r).html(e)})}function user_vote_review(e,n){$.post("/ajax/vote_product_review.php",{review_id:e,vote:n},function(e){if("error-not-login"==e)confirm("Để sử dụng chức năng này bạn cần đăng nhập. Click OK để đăng nhập")&&(window.location="/dang-nhap?return_url="+window.location);else if("error-has-voted"==e)alert("Bạn đã chọn rồi");else if("dislike"==n){if(message="Bạn không đồng ý với ý kiến này. Bạn có thể muốn viết ý kiến của mình không ?",confirm(message)){var r=window.location,t=encodeURI(r).replace("=review","=write-review");-1==t.search("=write-review")&&(t+="?section=write-review"),window.location=t}}else alert("Cảm ơn bạn đã bày tỏ quan điểm")})}function check_discount(e,n,r){"coupon"==e&&($("#checking_discount_code").html("vui lòng đợi..."),$.post("ajax/check_coupon.php",{code:n,order:r},function(e){var n=jQuery.parseJSON(e);if(""!=n.error)$("#checking_discount_code").html(n.error),$("#discount_code").val("");else{for(var r="",t=0,o=document.getElementById("total_value").innerHTML;o.indexOf(".")>0;)o=o.replace(".","");switch(n.type){case"pro":r=n.content;break;case"cash":r="Giảm "+writeStringToPrice(n.content+"")+" đ",t=parseInt(o)-parseInt(n.content),$("#total_value").html(writeStringToPrice(t+""));break;case"priceoff":r="Giảm "+n.content+"%",t=parseInt(o)*parseInt(100-n.content)/100,$("#total_value").html(writeStringToPrice(t+""));break;case"other":r=n.content}$("#checking_discount_code").html(n.title+": "+r),$("#discount_message").html(n.title+": "+r)}}))}function loadAjaxContent(e,n){if($("#anchor_top").length>0){var r=$("#anchor_top").offset(),t=r.top;$("html, body").animate({scrollTop:t},500)}$("#"+e).load(n)}function change_captcha(e){var n=(new Date).getTime();$("#"+e).attr("src","/includes/captcha/captcha.php?v="+n)}function user_cancel_order(e,n){return"new"!=n?(alert("Bạn chỉ hủy được đơn hàng chưa được xử lý. Vui lòng liên hệ bộ phận hỗ trợ của chúng tôi"),!1):void(confirm("Bạn chắc chắn muốn hủy đơn hàng này ?")&&$.post("/ajax/user_account.php",{action:"cancel-order",order_id:e,status:n},function(){alert("Bạn đã hủy đơn thành công !")}))}function user_vote_review(e,n){$.post("/ajax/vote_product_review.php",{review_id:e,vote:n},function(e){if("error-not-login"==e)confirm("Để sử dụng chức năng này bạn cần đăng nhập. Click OK để đăng nhập")&&(window.location="/dang-nhap?return_url="+window.location);else if("error-has-voted"==e)alert("Bạn đã chọn rồi");else if("dislike"==n){if(message="Bạn không đồng ý với ý kiến này. Bạn có thể muốn viết ý kiến của mình không ?",confirm(message)){var r=window.location,t=encodeURI(r).replace("=review","=write-review");-1==t.search("=write-review")&&(t+="?section=write-review"),window.location=t}}else alert("Cảm ơn bạn đã bày tỏ quan điểm")})}var Base64={_keyStr:"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",encode:function(e){var n,r,t,o,a,c,i,u="",l=0;for(e=Base64._utf8_encode(e);l<e.length;)n=e.charCodeAt(l++),r=e.charCodeAt(l++),t=e.charCodeAt(l++),o=n>>2,a=(3&n)<<4|r>>4,c=(15&r)<<2|t>>6,i=63&t,isNaN(r)?c=i=64:isNaN(t)&&(i=64),u=u+this._keyStr.charAt(o)+this._keyStr.charAt(a)+this._keyStr.charAt(c)+this._keyStr.charAt(i);return u},decode:function(e){var n,r,t,o,a,c,i,u="",l=0;for(e=e.replace(/[^A-Za-z0-9\+\/\=]/g,"");l<e.length;)o=this._keyStr.indexOf(e.charAt(l++)),a=this._keyStr.indexOf(e.charAt(l++)),c=this._keyStr.indexOf(e.charAt(l++)),i=this._keyStr.indexOf(e.charAt(l++)),n=o<<2|a>>4,r=(15&a)<<4|c>>2,t=(3&c)<<6|i,u+=String.fromCharCode(n),64!=c&&(u+=String.fromCharCode(r)),64!=i&&(u+=String.fromCharCode(t));return u=Base64._utf8_decode(u)},_utf8_encode:function(e){e=e.replace(/\r\n/g,"\n");for(var n="",r=0;r<e.length;r++){var t=e.charCodeAt(r);128>t?n+=String.fromCharCode(t):t>127&&2048>t?(n+=String.fromCharCode(t>>6|192),n+=String.fromCharCode(63&t|128)):(n+=String.fromCharCode(t>>12|224),n+=String.fromCharCode(t>>6&63|128),n+=String.fromCharCode(63&t|128))}return n},_utf8_decode:function(e){for(var n="",r=0,t=c1=c2=0;r<e.length;)t=e.charCodeAt(r),128>t?(n+=String.fromCharCode(t),r++):t>191&&224>t?(c2=e.charCodeAt(r+1),n+=String.fromCharCode((31&t)<<6|63&c2),r+=2):(c2=e.charCodeAt(r+1),c3=e.charCodeAt(r+2),n+=String.fromCharCode((15&t)<<12|(63&c2)<<6|63&c3),r+=3);return n}},default_price_currency="usd",SHOPPING_CART_URL="/gio-hang",ProductAddon={addCart:function(e,n){console.log("selected_addon ="+n),null==hura_read_cookie("shopping_cart_addon")&&hura_create_cookie("shopping_cart_addon",",",30);var r=hura_read_cookie("shopping_cart_addon");if(-1==r.search(","+e+":"+n+",")){var t=","+e+":"+n+r;hura_create_cookie("shopping_cart_addon",t,30)}else console.log("đã có trong giỏ hàng: "+r)},removeFromCart:function(e,n){var r=hura_read_cookie("shopping_cart_addon");if(null!=r&&-1!==r.search(","+e+":"+n)){var t=r.replace(","+e+":"+n,",");hura_create_cookie("shopping_cart_addon",t,30),window.location=SHOPPING_CART_URL}},removeProduct:function(e){var n=hura_read_cookie("shopping_cart_addon");if(null!=n){var r=new RegExp(","+e+":([0-9]+),","i"),t=n.replace(r,",");hura_create_cookie("shopping_cart_addon",t,30)}},checkSelect:function(e){var n=this;$(".product-addon-"+e).each(function(){if($(this).prop("checked")){var r=parseInt(this.value);r>0&&n.addCart(e,r)}})}};window.top!==window.self&&window.top.location.replace(window.self.location.href),$(function(){countShoppingCart("shopping_cart_store")});