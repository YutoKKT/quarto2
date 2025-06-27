using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class YoukanPutPieceAlgorithm : PutPieceAlgorithm
{
    public override Position putPiece(Board board)
    {
        // 今から設置しようとしているピース
        PieceId nextPutPeaceId = board.getSelectedPieceId();

        Position winPos = FindFirstPosition(board, nextPutPeaceId, (tempBoard) =>
        {
            // 勝者判定
            PlayerId winner = tempBoard.judge();
            return winner != PlayerId.None;
        });

        if (winPos.X != -1 && winPos.Y != -1)
        {
            // 勝てる場所が見つかったらそこに置く
            return winPos;
        }

        // 空いているマスの中からランダムに1つ選んで設置する
        return GetRandomEmptyPosition(board);
    }

    /// <summary>
    /// 盤面の全てのマスをチェックし、指定した条件を満たす最初の場所を返す
    /// 条件はFunc<Board, bool>で指定し、仮想的にピースを置いた後のBoardを渡す
    /// </summary>
    /// <param name="board">現在の盤面</param>
    /// <param name="pieceId">置くピースID</param>
    /// <param name="condition">条件（仮想盤面を受け取りboolを返す）</param>
    /// <returns>条件を満たす最初のPosition。なければ(-1,-1)</returns>
    private Position FindFirstPosition(Board board, PieceId pieceId, Func<Board, bool> condition)
    {
        // 盤面の全てのマスをチェック
        for (int i = 0; i < 16; i++)
        {
            // そのマスが空いているか確認
            if (board.getstate()[i] == null)
            {
                // 仮のボードを作成（ディープコピー）
                Board tempBoard = new Board();
                // 盤面の状態をコピー
                Piece[] currentState = board.getState();
                for (int j = 0; j < 16; j++)
                {
                    if (currentState[j] != null)
                    {
                        tempBoard.putPiece(currentState[j].getPieceId(), new Position(j % 4, j / 4));
                    }
                }
                // 選択可能な駒リストもコピー
                foreach (PieceId pid in board.getSelectablePieces())
                {
                    if (pid != pieceId)
                    {
                        // 既にputPieceで消費されていないものだけ追加
                        tempBoard.getSelectablePieces().Add(pid);
                    }
                }
                // 選択中のピースをセット
                tempBoard.setSelectedPiece(pieceId);

                // 仮のボードでピースを置く
                tempBoard.putPiece(pieceId, new Position(i % 4, i / 4));

                // 条件判定
                if (condition(tempBoard))
                {
                    return new Position(i % 4, i / 4);
                }
            }
        }
        return new Position(-1, -1);
    }

    /// <summary>
    /// 空いているマスの中からランダムに1つ選んで返す
    /// </summary>
    /// <param name="board">現在の盤面</param>
    /// <returns>ランダムな空きマスのPosition。なければ(-1,-1)</returns>
    private Position GetRandomEmptyPosition(Board board)
    {
        List<Position> emptyPositions = new List<Position>();
        for (int i = 0; i < board.getstate().Length; i++)
        {
            if (board.getstate()[i] == null)
            {
                emptyPositions.Add(new Position(i % 4, i / 4));
            }
        }
        if (emptyPositions.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, emptyPositions.Count);
            return emptyPositions[randomIndex];
        }
        return new Position(-1, -1);
    }
}