using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameBoardController : MonoBehaviour
{

    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private Tilemap highlightTilemap;
    private Tile pestTileObject;
    private Tile grassTileObject;

    private PestApplication application;

    private bool gameBoardChanged = true;

    public void Start()
    {
        // load the pest and grass tiles
        pestTileObject = TilesResourcesLoader.GetPestTile();
        grassTileObject = TilesResourcesLoader.GetGrassTile();

        // get the reference to application. 
        application = PestApplication.Instance;
    }

    public void Update()
    {
        // we only redraw if something has changes (and the world is initialized)
        if(gameBoardChanged & PestApplication.Instance.theWorld != null)
        {
            DrawMap();
            gameBoardChanged = false;
        }
    }

    private void DrawMap()
    {
        foreach(GridTile tile in application.theWorld.tileList)
        {
            Tile tileObject = null;
            switch(tile.type)
            {
                case GridTile.GridTileType.PEST:
                tileObject = pestTileObject;
                break;

                case GridTile.GridTileType.FARM:
                tileObject = TilesResourcesLoader.GetPlayerTile(tile.farmName);

                // TODO put the human farm highlighted
                if(new Location(tile.coordinates).Equals(PestApplication.Instance.theWorld.humanPlayer.farmLocation))
                {
                    Tile highlightObject = TilesResourcesLoader.GetHighlightTile();
                    this.highlightTilemap.SetTile(tile.coordinates, highlightObject);
                }
                else
                {
                    this.highlightTilemap.SetTile(tile.coordinates, null);
                }
                break;

                case GridTile.GridTileType.GRASS:
                tileObject = grassTileObject;
                break;
            }
            this.tilemap.SetTile(tile.coordinates, tileObject);
        }
    }

    public void GameBoardChanged()
    {
        this.gameBoardChanged = true;
    }


}