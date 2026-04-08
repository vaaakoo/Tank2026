using Tank2026.Core;

namespace Tank2026.Models;

public static class MapLoader
{
    private static readonly string[] Level1 = 
    {
        "@@@@@@@@@@@@@@@@@@@@",
        "@                  @",
        "@  # #  #  # #  #  @",
        "@  # #  #  # #  #  @",
        "@  # #  #  # #  #  @",
        "@  # #  #  # #  #  @",
        "@  ###  #  # ###   @",
        "@   #   #  #  #    @",
        "@  ###  #### ###   @",
        "@  # #       # #   @",
        "@  # #  #### # #   @",
        "@       #  #       @",
        "@       #B #       @",
        "@@@@@@@@@@@@@@@@@@@@"
    };

    private static readonly string[] Level2 = 
    {
        "@@@@@@@@@@@@@@@@@@@@",
        "@                  @",
        "@ #  #  ####  #  # @",
        "@ #  #  ####  #  # @",
        "@                    @",
        "@ ####  #  #  #### @",
        "@ ####  #  #  #### @",
        "@   ~   ####   ~   @",
        "@   ~          ~   @",
        "@   ~   ####   ~   @",
        "@       #  #       @",
        "@ %%    #B #    %% @",
        "@ %%    #  #    %% @",
        "@@@@@@@@@@@@@@@@@@@@"
    };

    private static readonly string[] Level3 = 
    {
        "@@@@@@@@@@@@@@@@@@@@",
        "@      @  @        @",
        "@   %  @  @   %    @",
        "@   %         %    @",
        "@   #  @  @   #    @",
        "@      @  @        @",
        "@  ~~~~~~~~~~~~~~~~@",
        "@  ~~~~~~~~~~~~~~~~@",
        "@                  @",
        "@  @@ @@ @@ @@ @@  @",
        "@  @@ @@ @@ @@ @@  @",
        "@        #  #      @",
        "@        #B #      @",
        "@@@@@@@@@@@@@@@@@@@@"
    };

    private static readonly string[] Level4 = 
    {
        "@@@@@@@@@@@@@@@@@@@@",
        "@######%  %######  @",
        "@######%  %######  @",
        "@      %  %        @",
        "@  ##  %  %  ##    @",
        "@  ##        ##    @",
        "@  ##   @@   ##    @",
        "@  ##   @@   ##    @",
        "@                  @",
        "@  @#@  ~~   @#@   @",
        "@  #@#       #@#   @",
        "@        ##        @",
        "@        #B#       @",
        "@@@@@@@@@@@@@@@@@@@@"
    };

    private static readonly string[] Level5 = 
    {
        "@@@@@@@@@@@@@@@@@@@@",
        "@#@#@#@#@#@#@#@#@#@@",
        "@                  @",
        "@ %%%%  ####  %%%% @",
        "@ %%%%  #  #  %%%% @",
        "@       #  #       @",
        "@       ####       @",
        "@ ~~~~        ~~~~ @",
        "@ ~~~~  @@@@  ~~~~ @",
        "@                  @",
        "@  #  #      #  #  @",
        "@  #  #  ##  #  #  @",
        "@  #  #  #B  #  #  @",
        "@@@@@@@@@@@@@@@@@@@@"
    };

    private static readonly string[][] Levels = { Level1, Level2, Level3, Level4, Level5 };

    public static int MaxLevels => Levels.Length;

    public static Map LoadLevel(int levelIndex)
    {
        // loop back to 0 if out of range
        var index = levelIndex % MaxLevels;
        var layout = Levels[index];
        
        var map = new Map(GameSettings.MapWidth, GameSettings.MapHeight);

        for (var y = 0; y < layout.Length && y < map.Height; y++)
        {
            var row = layout[y];
            for (var x = 0; x < row.Length && x < map.Width; x++)
            {
                var c = row[x];
                TileType type = c switch
                {
                    '#' => TileType.Brick,
                    '@' => TileType.Steel,
                    '~' => TileType.Water,
                    '%' => TileType.Grass,
                    'B' => TileType.Base,
                    _ => TileType.Empty
                };
                map.SetTile(x, y, type);
            }
        }
        return map;
    }
}
