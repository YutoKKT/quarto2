using System;
using System.Collections.Generic;
using UnityEngine;

public class YoukanSelectPieceAlgorithm : SelectPieceAlgorithm
{
    public override PieceId SelectPiece(Piece[] state)
    {
        // 盤面に存在しない（未配置）の駒IDリストを返す
        List<PieceId> selectablePieces = GetSelectablePieces(state);
        // 1手でクリアされてしまう駒を除外した安全な駒リストを返す
        List<PieceId> safePieces = GetSafePieces(state, selectablePieces);

        List<PieceId> finalCandidates = safePieces.Count > 0 ? safePieces : selectablePieces;

        return SelectRandomPiece(finalCandidates, safePieces.Count > 0);
    }

    /// <summary>
    /// 盤面に存在しない（未配置）の駒IDリストを返す
    /// </summary>
    private List<PieceId> GetSelectablePieces(Piece[] state)
    {
        PieceId[] allPieceIds = {
            PieceId.FSCB, PieceId.FSCW, PieceId.FSSB, PieceId.FSSW,
            PieceId.FTCB, PieceId.FTCW, PieceId.FTSB, PieceId.FTSW,
            PieceId.HSCB, PieceId.HSCW, PieceId.HSSB, PieceId.HSSW,
            PieceId.HTCB, PieceId.HTCW, PieceId.HTSB, PieceId.HTSW
        };

        List<PieceId> selectablePieces = new List<PieceId>();
        foreach (PieceId pid in allPieceIds)
        {
            if (IsPieceAvailable(state, pid))
            {
                selectablePieces.Add(pid);
            }
        }
        return selectablePieces;
    }

    /// <summary>
    /// 指定した駒IDが盤面に未配置かどうか判定
    /// </summary>
    private bool IsPieceAvailable(Piece[] state, PieceId pid)
    {
        for (int j = 0; j < state.Length; j++)
        {
            if (state[j] != null && state[j].getPieceId() == pid)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 1手でクリアされてしまう駒を除外した安全な駒リストを返す
    /// </summary>
    private List<PieceId> GetSafePieces(Piece[] state, List<PieceId> selectablePieces)
    {
        List<PieceId> safePieces = new List<PieceId>();
        foreach (PieceId candidate in selectablePieces)
        {
            if (!CanWinInOneMove(state, candidate))
            {
                safePieces.Add(candidate);
            }
        }
        return safePieces;
    }

    /// <summary>
    /// 指定した駒を相手に渡したとき、どこに置いても1手で勝てるかどうか判定
    /// </summary>
    private bool CanWinInOneMove(Piece[] state, PieceId candidate)
    {
        Board tempBoard = CreateBoardFromState(state);
        tempBoard.setSelectedPiece(candidate);

        for (int i = 0; i < 16; i++)
        {
            if (state[i] == null)
            {
                Board testBoard = CreateBoardFromState(tempBoard.getState());
                testBoard.setSelectedPiece(candidate);
                testBoard.putPiece(candidate, new Position(i % 4, i / 4));
                PlayerId winner = testBoard.judge();
                if (winner != PlayerId.None)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 盤面状態から新しいBoardインスタンスを作成
    /// </summary>
    private Board CreateBoardFromState(Piece[] state)
    {
        Board board = new Board();
        for (int i = 0; i < state.Length; i++)
        {
            if (state[i] != null)
            {
                board.putPiece(state[i].getPieceId(), new Position(i % 4, i / 4));
            }
        }
        return board;
    }

    /// <summary>
    /// 候補リストからランダムに1つ選んで返す
    /// </summary>
    private PieceId SelectRandomPiece(List<PieceId> candidates, bool hasSafe)
    {
        if (candidates.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, candidates.Count);
            PieceId selected = candidates[randomIndex];
            Debug.Log("CPUが駒を選択: " + selected + (hasSafe ? "" : "（安全駒なし）"));
            return selected;
        }
        Debug.LogError("選択可能な駒がありません");
        return 0;
    }
}