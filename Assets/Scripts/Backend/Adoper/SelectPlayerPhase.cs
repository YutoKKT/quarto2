using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 一人のプレイヤが手動か自動か選択するフェーズの後の処理
/// \n
/// なんでこの命名やねん
/// </summary>
public class SelectPlayerPhase : GamePhase
{
    private Dictionary<string,SelectPieceAlgorithm> selectPieceAlgorithms = new Dictionary<string, SelectPieceAlgorithm>();
    private Dictionary<string,PutPieceAlgorithm> putPieceAlgorithms = new Dictionary<string, PutPieceAlgorithm>();
    public SelectPlayerPhase()
    {
        type = GamePhaseType.SelectPlayer;
        //人が書いたアルゴリズムを手書きで追加
        selectPieceAlgorithms.Add("defo",new ConcreteSelectPieceAlgorithm());
        putPieceAlgorithms.Add("DEFO",new ConcretePutPieceAlgorithm());

        // 追加する
        selectPieceAlgorithms.Add("Youkan",new YoukanSelectPieceAlgorithm());
        putPieceAlgorithms.Add("Youkan",new YoukanPutPieceAlgorithm());
    }
    public override Result execute(Command command, GameController gameController)
    {
        PlayerInfo playerInfo = ((SelectPlayerCommand)command).playerInfo;
        
        Debug.Log("=== SelectPlayerPhase実行 ===");
        Debug.Log($"設定するプレイヤー情報: SelectPiece={playerInfo.SelectPiece}, PutPiece={playerInfo.PutPiece}");
        
        Result result = SelectPlayerUseCase.handle(playerInfo);
        gameController.setPlayerInfo(playerInfo);
        SelectPieceAlgorithm selectPieceAlgorithm = null;
        PutPieceAlgorithm putPieceAlgorithm = null;

        SelectPlayerCommand selectPlayerCommand = (SelectPlayerCommand)command;
        
        // アルゴリズム名のデフォルト値を設定
        string selectPieceAlgorithmName = "Youkan";
        string putPieceAlgorithmName = "Youkan";
        
        if (!string.IsNullOrEmpty(selectPlayerCommand.SelectPieceAlgorithmName))
        {
            selectPieceAlgorithmName = selectPlayerCommand.SelectPieceAlgorithmName;
        }
        
        if (!string.IsNullOrEmpty(selectPlayerCommand.PutPieceAlgorithmName))
        {
            putPieceAlgorithmName = selectPlayerCommand.PutPieceAlgorithmName;
        }
        
        Debug.Log($"アルゴリズム名: SelectPiece={selectPieceAlgorithmName}, PutPiece={putPieceAlgorithmName}");

        //playerのインスタンスを生成して，gameControllerにセット
        if(playerInfo.SelectPiece == PlayerType.Cpu)
        {
            Debug.Log("SelectPieceはCPU - アルゴリズム名: " + selectPieceAlgorithmName);
            selectPieceAlgorithm = selectPieceAlgorithms[selectPieceAlgorithmName];
        }
        else
        {
            Debug.Log("SelectPieceは人間プレイヤー");
        }
        
        if(playerInfo.PutPiece == PlayerType.Cpu)
        {
            Debug.Log("PutPieceはCPU - アルゴリズム名: " + putPieceAlgorithmName);
            putPieceAlgorithm = putPieceAlgorithms[putPieceAlgorithmName];
            
            // NULLチェック
            if (putPieceAlgorithm == null)
            {
                Debug.LogError("PutPieceAlgorithmがnullです！デフォルトアルゴリズムを使用します。");
                putPieceAlgorithm = new ConcretePutPieceAlgorithm();
            }
        }
        else
        {
            Debug.Log("PutPieceは人間プレイヤー");
        }
        
        Player player = new Player(selectPieceAlgorithm, putPieceAlgorithm);
        gameController.setPlayer(player);
        Debug.Log("プレイヤーオブジェクト作成完了");

        return result;
    }
    public override GamePhase getNextPhase(GameController gameController)
    {
        Debug.Log("=== SelectPlayerPhase: 次フェーズ決定 ===");
        Debug.Log($"現在の登録プレイヤー数: {gameController.playerInfos.Count}");
        
        if(gameController.playerInfos.Count < 2)
        {
            Debug.Log("まだプレイヤー2の設定が必要 → SelectPlayerPhaseを継続");
            return new SelectPlayerPhase();
        }
        else
        {
            Debug.Log("両プレイヤー設定完了 → ゲーム開始フェーズへ");
            
            // 全プレイヤー情報をデバッグ出力
            for(int i = 0; i < gameController.playerInfos.Count; i++)
            {
                var info = gameController.playerInfos[i];
                Debug.Log($"プレイヤー{i+1}: SelectPiece={info.SelectPiece}, PutPiece={info.PutPiece}");
            }
            
            PlayerType firstPlayerSelectType = gameController.getPlayerType(ActionType.SelectPiece);
            Debug.Log($"最初のプレイヤーのSelectPieceタイプ: {firstPlayerSelectType}");
            
            if(firstPlayerSelectType == PlayerType.Cpu)
            {
                Debug.Log("最初の駒選択はCPU → SelectPieceByCpuPhaseへ");
                return new SelectPieceByCpuPhase();
            }
            else
            {
                Debug.Log("最初の駒選択は人間 → SelectPieceByUserPhaseへ");
                return new SelectPieceByUserPhase();
            }
        }
    }
    public override Information getInformation(GameController gameController)
    {
        List<string> selectPieceAlgorithmNames = new List<string>();
        List<string> putPieceAlgorithmNames = new List<string>();
        foreach (var item in selectPieceAlgorithms)
        {
            selectPieceAlgorithmNames.Add(item.Key);
        }
        foreach (var item in putPieceAlgorithms)
        {
            putPieceAlgorithmNames.Add(item.Key);
        }

        return new SelectPlayerInformation(selectPieceAlgorithmNames,putPieceAlgorithmNames);
    }
}