using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameBoardController : MonoBehaviour
{

    [SerializeField]
    private Tilemap tilemap;
    private Tile pestTileObject;
    private Tile grassTileObject;

    private Application application;

    private bool gameBoardChanged = false;

    public void Start()
    {
        // load the pest and grass tiles
        pestTileObject = TilesResourcesLoader.GetPestTile();
        grassTileObject = TilesResourcesLoader.GetGrassTile();

        // get the reference to application. 
        application = Application.Instance;
        // At this moment, the world is loaded and ready (normally)
        // so we can draw it
        DrawMap();
    }

    public void Update()
    {
        // we only redraw if something has changes
        if(gameBoardChanged)
        {
            Debug.Log("Updating the map");
            DrawMap();
            gameBoardChanged = false;
        }
    }

    private void DrawMap()
    {
        foreach(GridTile tile in application.theWorld.tileList)
        {
            Debug.Log(tile);
            Tile tileObject = null;
            switch(tile.type)
            {
                case GridTile.GridTileType.PEST:
                tileObject = pestTileObject;
                break;

                case GridTile.GridTileType.FARM:
                tileObject = TilesResourcesLoader.GetPlayerTile(tile.farmName);
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
        Debug.Log("GameBoardcontroller.GameBoardChanged");
        this.gameBoardChanged = true;
    }


}