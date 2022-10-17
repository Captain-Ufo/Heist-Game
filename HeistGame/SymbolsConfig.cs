/////////////////////////////////
//Heist!, © Cristian Baldi 2022//
/////////////////////////////////

using System.Collections.Generic;

namespace HeistGame
{
    /// <summary>
    /// A helper class that holds in a centralized place all the gameplay relevant symbols
    /// </summary>
    public static class SymbolsConfig
    {
        public const char PlayerSymbol = '☻';
        public const char Health = '♥';
        public const char NPCMarkerUp = '^';
        public const char NPCMarkerRight = '>';
        public const char NPCMarkerDown = 'V';
        public const char NPCMarkerLeft = '<';

        public const char Exit = 'D';
        public const char ExitConfigSymbol = 'Ω';

        public const char Entrance = '≡';

        public const char Key = '¶';

        public const char Spawn = '@';

        public const char Empty = ' ';

        public const char Treasure = '$';

        public const char LeverOff = '\\';
        public const char LeverOn = '/';
        public const char Gate = '#';

        public const char TransparentWallVertical = ':';
        public const char TransparentWallHorizontal = '·';

        public const char EnclosedSpace = '.';
        public const char StrongLight = '*';
        public const char WeakLight = '+';
        public const char Light0 = '·';
        public const char Light1 = '░';
        public const char Light2 = '▒';
        public const char Light3 = '▓';

        public const char Signpost = '?';

        public const char WallsMap = '!';
        public const char ExitMap = '¡';
        public const char ObjectivesMap = '¿';
        public const char FullMap = '?';

        public const char VerticalDoorVisual = '|';
        public const char VerticalDoorOpen = '¦';
        public const char VerticalDoorLock1 = '▎';
        public const char VerticalDoorLock2 = '▌';
        public const char VerticalDoorLock3 = '█';

        public const char HorizontalDoorVisual = '-';
        public const char HorizontalDoorOpen = '~';
        public const char HorizontalDoorLock1 = '÷';
        public const char HorizontalDoorLock2 = '=';
        public const char HorizontalDoorLock3 = '±';

        public const char ChestClosed = '◙';
        public const char ChestOpened = '◘';
        public const char ChestEmpty = 'Ɔ';
        public const char ChestWithTreasure = 'Ͼ';
        public const char ChestWithRandomTresture = 'Ͽ';            

        public const string OutroArt = @"


















                          _.--.         
                      _.-'_:-'||        
                  _.-'_.-::::'||        
             _.-:'_.-::::::'  ||        
           .'`-.-:::::::'     ||             .='''_;=._                     
          /.'`;|:::::::'      ||_         ,-'_,=''     `'=.                 
         ||   ||::::::'     _.;._'-._     '=._o`'-._        `'=.            
         ||   ||:::::'  _.-!oo @.!-._'-.       `'=._o`'=._ _     `'=._      
         \'.  ||:::::.-!()oo @!()@.-'_.|          __:=._o '=._.'_.-='''=.   
          '.'-;|:.-'.&$@.& ()$%-'o.'\U||    __.--' , ; `'=._o.' ,-'''-._ '. 
            `>'-.!@%()@'@_%-'_.-o _.|'|| ._'  ,. .`@` `` ,  `'-._'-._   '. |
             ||-._'-.@.-'_.-' _.-o  |'|| |o`'=._`@, °`@`; .'. ,  '-._'-._; ;
             ||=[ '-._.-\U/.-'    o_..--.._;`-.o`'=._; .O `*'`.§\` . '-._ / 
             || '-.]=|| |'|      o';-.--_';'    `'-.o`'=._``()'` @ ,__.--o; 
             ||      || |'|        _}===={ ;     (#) `-.o `'=.`_.--'_o.-; ; 
             ||      || |'|    _.-.'  _|_ '.._o--._        ; | ;        ; ; 
             |'-._   || |'|_.-'_./:: (_|_   \  '=._o--._   ;o|o;     _._;o; 
              '-._'-.|| |' `_.-'|::  ,_|_)   |      '=._o._; | ;_.--'o.--'  
                  '-.||_/.-'°   \::.   |     /     @     '=.o|o_.--''       
              o            0  @ o'::_     _-'  o       *                    
                             @  *@   ''-'O oO                               
";

        public const string Castle = @"
  
                                         o
                                    .-'''|
                                    |,-''|
                                         |   _.- '`.
                                        _|-''_.-'|. `.
                                       |:^.-'_.-'`.;. `.
                                       | `.'.   ,-'_.-'|
                                       |   + '-'.-'   J
                    __.            .d88|    `.-'      |
               _.--'_..`.    .d88888888|     |       J'b.
            +:' ,--'_.|`.`.d88888888888|-.   |    _-.|888b.
            | \ \-'_.--'_.-+888888888+'  _>F F +:'   `88888bo.
             L \ +'_.--'   |88888+''  _.' J J J  `.    +8888888b.
             |  `+'        |8+''  _.-'    | | |    +    `+8888888._-'.
           .d8L L         J _.-'          | | |     `.    `+888+^'.-|.`.
          d888|  |         J-'            F F F       `.  _.-'_.-'_.+.`.`.
         d88888L L     _.   L            J J J          `|. +'_.-'    `_+ `;
         888888J  |  +-'  \ L         _.-+.|.+.          F `.`.     .-'_.-'J
         8888888|  L L\    \|     _.-'     '   `.       J    `.`.,-'.-'    |
         8888888PL | | \    `._.-'               `.     |      `..-'      J.b
         8888888 |  L L `.    \     _.-+.          `.   L+`.     |        F88b
         8888888  L | |   \   _..--'_.-|.`.          >-'    `., J        |8888b
         8888888  |  L L   +:' _.--'_.-'.`.`.    _.-'     .-' | |       JY88888b
         8888888   L | |   J \ \_.-'     `.`.`.-'     _.-'   J J        F Y88888b
         Y888888    \ L L   L \ `.      _.-'_.-+  _.-'       | |       |   Y88888b
         `888888b    \| |   |  `. \ _.-'_.-'   |-'          J J       J     Y88888b
          Y888888     +'\   J    \ '_.-'       F    ,-T'\   | |    .-'      )888888
           Y88888b.      \   L    +'          J    /  | J  J J  .-'        .d888888
            Y888888b      \  |    |           |    F  '.|.-'+|-'         .d88888888
             Y888888b      \ J    |           F   J    -.              .od88888888P
              Y888888b      \ L   |          J    | .' ` \d8888888888888888888888P
               Y888888b      \|   |          |  .-'`.  `\ `.88888888888888888888P
                Y888888b.     J   |          F-'     \\ ` \ \88888888888888888P'
                 Y8888888b     L  |         J     d8`.`\  \`.8888888888888P'
                  Y8888888b    |  |        .+      d8888\  ` .'  `Y888888P'
                  `88888888b   J  |     .-'     .od888888\.-'
                   Y88888888b   \ |  .-'     d888888888P'
                   `888888888b   \|-'       d888888888P       ,0\
                    `Y88888888b            d8888888P'        ,\'^6
                      Y88888888bo.      .od88888888 hs         988b
                      `8888888888888888888888888888              §8
                       Y88888888888888888888888888P                °
                       `Y8888888888888888888888P'
                         `Y8888888888888P'
                              `Y88888P'";
    }
}
