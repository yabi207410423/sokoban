using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

public class SKBfor12 : MonoBehaviour
{
    GameObject cc;
    GameObject fc;
    GameObject PC;

    // タイルの種類
    private enum TileType
    {
        NONE, // 何も無い
        GROUND, // 地面
        TARGET_1, // 目的地
        TARGET_2,
        TARGET_3,
        PLAYER, // プレイヤー
        BLOCK, // 
        

        PLAYER_ON_TARGET_1, // プレイヤー（目的地の上）
        PLAYER_ON_TARGET_2,
        PLAYER_ON_TARGET_3,
        BLOCK_ON_TARGET_1, // ブロック（目的地の上）
        BLOCK_ON_TARGET_2,
        BLOCK_ON_TARGET_3
    }

    // 方向の種類
    private enum DirectionType
    {
        UP, // 上
        RIGHT, // 右
        DOWN, // 下
        LEFT, // 左
    }

    

    public TextAsset stageFile; // ステージ構造が記述されたテキストファイル

    private int rows; // 行数
    private int columns; // 列数
    
    public int pace;
    public int LEVEL;
    private TileType[,] tileList; // タイル情報を管理する二次元配列

    public float tileSize; // タイルのサイズ

    public Sprite groundSprite; // 地面のスプライト
    public Sprite targetSprite; // 目的地のスプライト
    public Sprite playerSprite_f; // プレイヤーのスプライト
    public Sprite playerSprite_b;
    public Sprite playerSprite_l;
    public Sprite playerSprite_r;
    public Sprite blockSprite; // ブロックのスプライト

    public Text PaceCount;
    public AudioSource WalkSound;
    public AudioSource BlockSound;

    private GameObject player; // プレイヤーのゲームオブジェクト
    private Vector2 middleOffset; // 中心位置
    private int blockCount; // ブロックの数

    public static bool isClear; // ゲームをクリアした場合 true
    private bool playerOnTarget_1 = false;
    private bool playerOnTarget_2 = false;
    private bool playerOnTarget_3 = false;

    // 各位置に存在するゲームオブジェクトを管理する連想配列
    private Dictionary<GameObject, Vector2Int> gameObjectPosTable = new Dictionary<GameObject, Vector2Int>();

    
    // ゲーム開始時に呼び出される
    public GameObject canvas;
    private void Start()
    {
        if (File.Exists(Application.dataPath + "/Save.txt"))
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/Save.txt");
            string exm = sr.ReadLine();
            sr.Close();
            if (exm == null)
            {
                FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Create);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(0);
                }
                fs.Close();
            }            
        }
        else
        {
            FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(0);
            }
            fs.Close();
        }

        PaceCount.text = pace.ToString();
        cc = GameObject.Find("CC");
        fc = GameObject.Find("FC");
        cc.SetActive(false);
        fc.SetActive(false);
        isClear= false;
        LoadTileData(); // タイルの情報を読み込む
        CreateStage(); // ステージを作成
    }

    // タイルの情報を読み込む
    private void LoadTileData()
    {
        // タイルの情報を一行ごとに分割
        var lines = stageFile.text.Split
        (
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

        // タイルの列数を計算
        var nums = lines[ 0 ].Split( new[] { ',' } );

        // タイルの列数と行数を保持
        rows = lines.Length; // 行数
        columns = nums.Length; // 列数

        // タイル情報を int 型の２次元配列で保持
        tileList = new TileType[ columns, rows ];
        for ( int y = 0; y < rows; y++ )
        {
            // 一文字ずつ取得
            var st = lines[ y ];
            nums = st.Split( new[] { ',' } );
            for ( int x = 0; x < columns; x++ )
            {
                // 読み込んだ文字を数値に変換して保持
                tileList[ x, y ] = ( TileType )int.Parse( nums[ x ] );
            }
        }
    }

    // ステージを作成
    private void CreateStage()
    {
        // ステージの中心位置を計算
        middleOffset.x = columns * tileSize * 0.5f - tileSize * 0.5f;
        middleOffset.y = rows * tileSize * 0.5f - tileSize * 0.5f; ;

        for ( int y = 0; y < rows; y++ )
        {
            for ( int x = 0; x < columns; x++ )
            {
                var val = tileList[ x, y ];

                // 何も無い場所は無視
                if ( val == TileType.NONE ) continue;

                // タイルの名前に行番号と列番号を付与
                var name = "tile" + y + "_" + x;

                // タイルのゲームオブジェクトを作成
                var tile = new GameObject( name );

                // タイルにスプライトを描画する機能を追加
                var sr = tile.AddComponent<SpriteRenderer>();

                
                
                // タイルのスプライトを設定
                sr.sprite = groundSprite;

                // タイルの位置を設定
                tile.transform.position = GetDisplayPosition( x, y );

                // 目的地の場合
                if ( val == TileType.TARGET_1 || val == TileType.TARGET_2 || val == TileType.TARGET_3)
                {
                    // 目的地のゲームオブジェクトを作成
                    var destination = new GameObject( "destination" );

                    // 目的地にスプライトを描画する機能を追加
                    sr = destination.AddComponent<SpriteRenderer>();

                    // 目的地のスプライトを設定
                    sr.sprite = targetSprite;

                    // 目的地の描画順を手前にする
                    sr.sortingOrder = 1;

                    // 目的地の位置を設定
                    destination.transform.position = GetDisplayPosition( x, y );
                }
                
                if ( val == TileType.PLAYER )
                {
                    // プレイヤーのゲームオブジェクトを作成
                    player = new GameObject( "player" );

                    // プレイヤーにスプライトを描画する機能を追加
                    sr = player.AddComponent<SpriteRenderer>();

                    // プレイヤーのスプライトを設定
                    sr.sprite = playerSprite_f;

                    // プレイヤーの描画順を手前にする
                    sr.sortingOrder = 2;

                    // プレイヤーの位置を設定
                    player.transform.position = GetDisplayPosition( x, y );

                    // プレイヤーを連想配列に追加
                    gameObjectPosTable.Add( player, new Vector2Int( x, y ) );
                }
                // ブロックの場合
                else if ( val == TileType.BLOCK )
                {
                    // ブロックの数を増やす
                    blockCount++;

                    // ブロックのゲームオブジェクトを作成
                    var block = new GameObject( "block" + blockCount );

                    // ブロックにスプライトを描画する機能を追加
                    sr = block.AddComponent<SpriteRenderer>();

                    // ブロックのスプライトを設定
                    sr.sprite = blockSprite;

                    // ブロックの描画順を手前にする
                    sr.sortingOrder = 2;

                    // ブロックの位置を設定
                    block.transform.position = GetDisplayPosition( x, y );

                    // ブロックを連想配列に追加
                    gameObjectPosTable.Add( block, new Vector2Int( x, y ) );
                }
                
            }
        }
    }

    // 指定された行番号と列番号からスプライトの表示位置を計算して返す
    private Vector2 GetDisplayPosition( int x, int y )
    {
        return new Vector2
        (
            x *  tileSize - middleOffset.x,
            y * -tileSize + middleOffset.y
        );
    }

    // 指定された位置に存在するゲームオブジェクトを返します
    private GameObject GetGameObjectAtPosition( Vector2Int pos )
    {
        foreach ( var pair in gameObjectPosTable )
        {
            // 指定された位置が見つかった場合
            if ( pair.Value == pos )
            {
                // その位置に存在するゲームオブジェクトを返す
                return pair.Key;
            }
        }
        return null;
    }

    // 指定された位置のタイルがブロックなら true を返す
    private bool IsBlock( Vector2Int pos )
    {
        var cell = tileList[ pos.x, pos.y ];
        return cell == TileType.BLOCK || cell == TileType.BLOCK_ON_TARGET_1 || cell == TileType.BLOCK_ON_TARGET_2 || cell == TileType.BLOCK_ON_TARGET_3;
    }

    // 指定された位置がステージ内なら true を返す
    private bool IsValidPositionForPlayer( Vector2Int pos )
    {
        
        if ( 0 <= pos.x && pos.x < columns && 0 <= pos.y && pos.y < rows )
        {
            return tileList[ pos.x, pos.y ] == TileType.GROUND || tileList[ pos.x, pos.y ] == TileType.BLOCK || tileList[ pos.x, pos.y ] == TileType.TARGET_1 || tileList[pos.x, pos.y] == TileType.TARGET_2 || tileList[pos.x, pos.y] == TileType.TARGET_3;
        }
        return true;
    }
    private bool IsValidPositionForBlock(Vector2Int pos)
    {

        if (0 <= pos.x && pos.x < columns && 0 <= pos.y && pos.y < rows)
        {
            return tileList[pos.x, pos.y] == TileType.GROUND || tileList[pos.x, pos.y] == TileType.BLOCK;
        }
        return true;
    }

    // 毎フレーム呼び出される
    private void Update()
    {
        if ( isClear ){
            cc.SetActive(true);
            StreamReader sr = new StreamReader(Application.dataPath + "/Save.txt");
            int tem = int.Parse(sr.ReadLine());
            sr.Close();
            if (tem < LEVEL+1)
            {
                SaveTheGame();
            }
            return;
        } 
        else if(pace==0){
            fc.SetActive(true);
            return;
        }
        // ゲームクリアしている場合は操作できないようにする
        

        // 上矢印が押された場合
        if ( Input.GetKeyDown( KeyCode.UpArrow ) )
        {
            // プレイヤーが上に移動できるか検証
            TryMovePlayer( DirectionType.UP );            
        }
        // 右矢印が押された場合
        else if ( Input.GetKeyDown( KeyCode.RightArrow ) )
        {
            // プレイヤーが右に移動できるか検証
            TryMovePlayer( DirectionType.RIGHT );            
        }
        // 下矢印が押された場合
        else if ( Input.GetKeyDown( KeyCode.DownArrow ) )
        {
            // プレイヤーが下に移動できるか検証
            TryMovePlayer( DirectionType.DOWN );            
        }
        // 左矢印が押された場合
        else if ( Input.GetKeyDown( KeyCode.LeftArrow ) )
        {
            // プレイヤーが左に移動できるか検証
            TryMovePlayer( DirectionType.LEFT );                       
        }
    }

    // 指定された方向にプレイヤーが移動できるか検証
    // 移動できる場合は移動する
    private void TryMovePlayer( DirectionType direction )
    {
        // プレイヤーの現在地を取得
        var currentPlayerPos = gameObjectPosTable[ player ];

        // プレイヤーの移動先の位置を計算
        var nextPlayerPos = GetNextPositionAlong( currentPlayerPos, direction );

        // プレイヤーの移動先がステージ内ではない場合は無視
        if ( !IsValidPositionForPlayer( nextPlayerPos ) ) return;
        // プレイヤーの移動先にブロックが存在する場合
        if ( IsBlock( nextPlayerPos ) )
        {
            // ブロックの移動先の位置を計算
            var nextBlockPos = GetNextPositionAlong( nextPlayerPos, direction );
            // ブロックの移動先がステージ内の場合かつ
            // ブロックの移動先にブロックが存在しない場合
            if (IsValidPositionForBlock( nextBlockPos ) && !IsBlock( nextBlockPos ) )
            {
                // 移動するブロックを取得
                var block = GetGameObjectAtPosition( nextPlayerPos );

                // プレイヤーの移動先のタイルの情報を更新
                UpdateGameObjectPosition( nextPlayerPos );

                // ブロックを移動
                block.transform.position = GetDisplayPosition( nextBlockPos.x, nextBlockPos.y );

                // ブロックの位置を更新
                gameObjectPosTable[ block ] = nextBlockPos;

                // ブロックの移動先の番号を更新
                if ( tileList[ nextBlockPos.x, nextBlockPos.y ] == TileType.GROUND )
                {
                    // 移動先が地面ならブロックの番号に更新
                    tileList[ nextBlockPos.x, nextBlockPos.y ] = TileType.BLOCK;
                    BlockSound.Play();
                }
                else if (tileList[nextBlockPos.x, nextBlockPos.y] == TileType.TARGET_1)
                {
                    // 移動先が目的地ならブロック（目的地の上）の番号に更新
                    tileList[ nextBlockPos.x, nextBlockPos.y] = TileType.BLOCK_ON_TARGET_1;
                }
                else if (tileList[nextBlockPos.x, nextBlockPos.y] == TileType.TARGET_2)
                {
                    // 移動先が目的地ならブロック（目的地の上）の番号に更新
                    tileList[nextBlockPos.x, nextBlockPos.y] = TileType.BLOCK_ON_TARGET_2;
                }
                else if (tileList[nextBlockPos.x, nextBlockPos.y] == TileType.TARGET_3)
                {
                    // 移動先が目的地ならブロック（目的地の上）の番号に更新
                    tileList[nextBlockPos.x, nextBlockPos.y] = TileType.BLOCK_ON_TARGET_3;
                }

                // プレイヤーの現在地のタイルの情報を更新
                UpdateGameObjectPosition( currentPlayerPos );

                // プレイヤーを移動
                player.transform.position = GetDisplayPosition( nextPlayerPos.x, nextPlayerPos.y );

                // プレイヤーの位置を更新
                gameObjectPosTable[ player ] = nextPlayerPos;

                // プレイヤーの移動先の番号を更新
                if ( tileList[ nextPlayerPos.x, nextPlayerPos.y ] == TileType.GROUND )
                {
                    // 移動先が地面ならプレイヤーの番号に更新
                    tileList[ nextPlayerPos.x, nextPlayerPos.y ] = TileType.PLAYER;
                    PaceDown();
                    WalkSound.Play();
                }
                else if ( tileList[ nextPlayerPos.x, nextPlayerPos.y ] == TileType.TARGET_1 )
                {
                    // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                    tileList[ nextPlayerPos.x, nextPlayerPos.y ] = TileType.PLAYER_ON_TARGET_1;
                    PaceDown();
                    WalkSound.Play();
                }
                else if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.TARGET_2)
                {
                    // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                    tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER_ON_TARGET_2;
                    PaceDown();
                    WalkSound.Play();
                }
                else if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.TARGET_3)
                {
                    // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                    tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER_ON_TARGET_3;
                    PaceDown();
                    WalkSound.Play();
                }
            }
        }
        // プレイヤーの移動先にブロックが存在しない場合
        else
        {
            // プレイヤーの現在地のタイルの情報を更新
            UpdateGameObjectPosition( currentPlayerPos );

            // プレイヤーを移動
            player.transform.position = GetDisplayPosition( nextPlayerPos.x, nextPlayerPos.y );

            // プレイヤーの位置を更新
            gameObjectPosTable[ player ] = nextPlayerPos;

            // プレイヤーの移動先の番号を更新
            if ( tileList[ nextPlayerPos.x, nextPlayerPos.y ] == TileType.GROUND)
            {
                // 移動先が地面ならプレイヤーの番号に更新
                tileList[ nextPlayerPos.x, nextPlayerPos.y ] = TileType.PLAYER;
                PaceDown();
                WalkSound.Play();
            }
            else if ( tileList[ nextPlayerPos.x, nextPlayerPos.y ] == TileType.TARGET_1 )
            {
                // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                tileList[ nextPlayerPos.x, nextPlayerPos.y ] = TileType.PLAYER_ON_TARGET_1;
                PaceDown();
                WalkSound.Play();
            }
            else if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.TARGET_2)
            {
                // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER_ON_TARGET_2;
                PaceDown();
                WalkSound.Play();
            }
            else if (tileList[nextPlayerPos.x, nextPlayerPos.y] == TileType.TARGET_3)
            {
                // 移動先が目的地ならプレイヤー（目的地の上）の番号に更新
                tileList[nextPlayerPos.x, nextPlayerPos.y] = TileType.PLAYER_ON_TARGET_3;
                PaceDown();
                WalkSound.Play();
            }
        }

        // ゲームをクリアしたかどうか確認
        CheckCompletion();
    }

    // 指定された方向の位置を返す
    private Vector2Int GetNextPositionAlong( Vector2Int pos, DirectionType direction )
    {
        
        switch ( direction )
        {
            // 上
            case DirectionType.UP:
                pos.y -= 1;
                player.GetComponent<SpriteRenderer>().sprite = playerSprite_b;
                break;
            // 右
            case DirectionType.RIGHT:
                pos.x += 1;
                player.GetComponent<SpriteRenderer>().sprite = playerSprite_r;
                break;
            // 下
            case DirectionType.DOWN:
                pos.y += 1;
                player.GetComponent<SpriteRenderer>().sprite = playerSprite_f;
                break;
            // 左
            case DirectionType.LEFT:
                pos.x -= 1;
                player.GetComponent<SpriteRenderer>().sprite = playerSprite_l;
                break;
        }
        return pos;
    }

    // 指定された位置のタイルを更新
    private void UpdateGameObjectPosition( Vector2Int pos )
    {
        // 指定された位置のタイルの番号を取得
        var cell = tileList[ pos.x, pos.y ];

        // プレイヤーもしくはブロックの場合
        if ( cell == TileType.PLAYER || cell == TileType.BLOCK )
        {
            // 地面に変更
            tileList[ pos.x, pos.y ] = TileType.GROUND;
        }
        // 目的地に乗っているプレイヤーもしくはブロックの場合
        else if (cell == TileType.PLAYER_ON_TARGET_1 || cell == TileType.BLOCK_ON_TARGET_1)
        {
            // 目的地に変更
            tileList[ pos.x, pos.y ] = TileType.TARGET_1;
        }
        else if (cell == TileType.PLAYER_ON_TARGET_2 || cell == TileType.BLOCK_ON_TARGET_2)
        {
            // 目的地に変更
            tileList[pos.x, pos.y] = TileType.TARGET_2;
        }
        else if (cell == TileType.PLAYER_ON_TARGET_3 || cell == TileType.BLOCK_ON_TARGET_3)
        {
            // 目的地に変更
            tileList[pos.x, pos.y] = TileType.TARGET_3;
        }
    }

    // ゲームをクリアしたかどうか確認
    private void CheckCompletion()
    {
        // 目的地に乗っているブロックの数を計算
        

        for ( int y = 0; y < rows; y++ )
        {
            for ( int x = 0; x < columns; x++ )
            {
                if ( tileList[ x, y ] == TileType.PLAYER_ON_TARGET_1 )
                {
                    playerOnTarget_1 = true;
                }
                else if (tileList[x, y] == TileType.PLAYER_ON_TARGET_2)
                {
                    playerOnTarget_2 = true;
                }
                else if (tileList[x, y] == TileType.PLAYER_ON_TARGET_3)
                {
                    playerOnTarget_3 = true;
                }
            }
        }

        // すべてのブロックが目的地の上に乗っている場合
        if (playerOnTarget_1 && playerOnTarget_2 && playerOnTarget_3)
        {
            // ゲームクリア
            isClear = true;
        }
    }
    public void SaveTheGame()
    {
        FileStream fs = new FileStream(Application.dataPath + "/Save.txt", FileMode.Create);
        using (StreamWriter sw = new StreamWriter(fs))
        {
            sw.Write(LEVEL+1);
        }
        fs.Close();
    }
    public void PaceDown(){
        pace -= 1;
        PaceCount.text = pace.ToString();
    }
}