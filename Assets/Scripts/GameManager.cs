using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    

    [Header("Setup piece")]
    [SerializeField] private List<Piece> allPieces = new List<Piece>();

    [Header("UI references")]
    [SerializeField] private GameObject winPanel;

    [SerializeField] private TextMeshProUGUI WinText;

    private int snappedPiecesCount = 0;

    


    void Start()
    {
        // Find all pieces if not manually assigned
        //if (allPieces.Count == 0)
        //{
        //    Piece[] pieces = FindObjectsOfType<Piece>();
        //    allPieces.AddRange(pieces);
        //}

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        Piece.OnPieceSnapped += OnPieceSnapped;
    }

    public void OnPieceSnapped(Piece piece)
    {
        snappedPiecesCount++;

        Debug.Log($"Pieces snapped: {snappedPiecesCount}/{allPieces.Count}");

        // Check if puzzle is complete
        if (snappedPiecesCount >= allPieces.Count)
        {
            OnPuzzleComplete();
        }
    }

    void OnPuzzleComplete()
    {
        Debug.Log("Puzzle Complete! Congratulations!");
        WinText.text = "Puzzle Complete! Congratulations!";

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    public void ResetPuzzle()
    {
        snappedPiecesCount = 0;

        foreach (Piece piece in allPieces)
        {
            piece.ResetPiece();
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void OnDisable()
    {
        Piece.OnPieceSnapped -= OnPieceSnapped;
    }
}
