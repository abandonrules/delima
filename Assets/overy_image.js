 var BackgroundTex : Texture2D;
 var ForegroundTex : Texture2D;
 private var newTex : Texture2D;
 var sound : AudioClip;
 
 function Start () {
     newTex = new Texture2D(BackgroundTex.width, BackgroundTex.height, TextureFormat.ARGB32, false);
     CombineTextures(newTex, BackgroundTex, ForegroundTex);
 }
 
 function CombineTextures (newTexture : Texture2D, Background : Texture2D, Overlay : Texture2D) {
     var offset = Vector2(((newTexture.width - Overlay.width)/2), ((newTexture.height - Overlay.height)/2));
     
     newTexture.SetPixels(Background.GetPixels());
     
     for(var y : int = 0; y < Overlay.height; y++){
         for(var x : int = 0; x < Overlay.width; x++){
             var PixelColorFore = Overlay.GetPixel(x, y)*Overlay.GetPixel(x, y).a;
             var PixelColorBack = newTexture.GetPixel(x + offset.x, y + offset.y)*(1-PixelColorFore.a);
             newTexture.SetPixel(x + offset.x, y + offset.y, PixelColorBack + PixelColorFore);
         }
     }
     
     newTexture.Apply();
 }