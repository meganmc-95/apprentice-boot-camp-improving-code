using System;
using System.Collections.Generic;
using System.Linq;

namespace TriviaGame
{
    public class Game
    {
        private readonly List<string> players = new List<string>();

        private const int maxBoardSpaces = 12;
        private const int maxPlayerCount = 6;
        private const int questionCountPerCategory = 50;
        private const int winningScore = 6;
        private readonly int[] playerPositions = new int[maxPlayerCount];
        private readonly int[] playerScores = new int[maxPlayerCount];

        private readonly bool[] inPenaltyBox = new bool[maxPlayerCount];

        private readonly LinkedList<string> popQuestions = new LinkedList<string>();
        private readonly LinkedList<string> scienceQuestions = new LinkedList<string>();
        private readonly LinkedList<string> sportsQuestions = new LinkedList<string>();
        private readonly LinkedList<string> rockQuestions = new LinkedList<string>();

        private int currentPlayer;
        private bool isGettingOutOfPenaltyBox;
        
        public Game()
        {
            for (var questionIndex = 0; questionIndex < questionCountPerCategory; questionIndex++)
            {
                popQuestions.AddLast(CreateQuestion("Pop", questionIndex));
                scienceQuestions.AddLast(CreateQuestion("Science", questionIndex));
                sportsQuestions.AddLast(CreateQuestion("Sports", questionIndex));
                rockQuestions.AddLast(CreateQuestion("Rock", questionIndex));
            }
        }

        public string CreateQuestion(string category, int index)
        {
            return $"{category} Question " + index;
        }

        public bool Add(string playerName)
        {
            players.Add(playerName);
            playerPositions[players.Count] = 0;
            playerScores[players.Count] = 0;
            inPenaltyBox[players.Count] = false;

            Console.WriteLine(playerName + " was added");
            Console.WriteLine("They are player number " + players.Count);
            return true;
        }

        public void Roll(int numberRolled)
        {
            Console.WriteLine(players[currentPlayer] + " is the current player");
            Console.WriteLine("They have rolled a " + numberRolled);

            if (inPenaltyBox[currentPlayer])
            {
                if (!IsOdd(numberRolled))
                {
                    Console.WriteLine(players[currentPlayer] + " is not getting out of the penalty box");
                    return;
                }
                Console.WriteLine(players[currentPlayer] + " is getting out of the penalty box");
            }

            PlayerTurn(numberRolled);
        }

        private bool IsOdd(int numberRolled)
        {
            var isOdd = numberRolled % 2 != 0;
            isGettingOutOfPenaltyBox = isOdd;
            return isOdd;
        }

        private void PlayerTurn(int numberRolled)
        {
            SetNewPlayerPosition(numberRolled);

            Console.WriteLine("The category is " + CurrentCategory());
            AskQuestion();
        }

        private void SetNewPlayerPosition(int numberRolled)
        {
            playerPositions[currentPlayer] += numberRolled;
            if (playerPositions[currentPlayer] > 11) playerPositions[currentPlayer] -= maxBoardSpaces;

            Console.WriteLine(players[currentPlayer]
                              + "'s new location is "
                              + playerPositions[currentPlayer]);
        }

        private void AskQuestion()
        {
            switch (CurrentCategory())
            {
                case "Pop":
                    AskCategoryQuestion(popQuestions);
                    break;
                case "Science":
                    AskCategoryQuestion(scienceQuestions);
                    break;
                case "Sports":
                    AskCategoryQuestion(sportsQuestions);
                    break;
                case "Rock":
                    AskCategoryQuestion(rockQuestions);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private void AskCategoryQuestion(LinkedList<string> questions)
        {
            Console.WriteLine(questions.First());
            questions.RemoveFirst();
        }

        private string CurrentCategory()
        {
            switch (playerPositions[currentPlayer])
            {
                case 0:
                case 4:
                case 8:
                    return "Pop";
                case 1:
                case 5:
                case 9:
                    return "Science";
                case 2:
                case 6:
                case 10:
                    return "Sports";
                default:
                    return "Rock";
            }
        }

        public bool WasCorrectlyAnswered()
        {
            if (inPenaltyBox[currentPlayer] && !isGettingOutOfPenaltyBox) 
            {
                    MoveToNextPlayer();
                    return false;
            }
            
            IncrementScore();

            bool didPlayerWin = DidPlayerWin();
            MoveToNextPlayer();

            return didPlayerWin;
        }

        public bool WasIncorrectlyAnswered()
        {
            Console.WriteLine("Question was incorrectly answered");
            Console.WriteLine(players[currentPlayer] + " was sent to the penalty box");
            inPenaltyBox[currentPlayer] = true;

            currentPlayer++;
            if (currentPlayer == players.Count) currentPlayer = 0;
            return false;
        }

        private void MoveToNextPlayer()
        {
            currentPlayer++;
            if (currentPlayer == players.Count) currentPlayer = 0;
        }

        private void IncrementScore()
        {
            Console.WriteLine("Answer was correct!!!!");
            playerScores[currentPlayer]++;
            Console.WriteLine(players[currentPlayer]
                              + " now has "
                              + playerScores[currentPlayer]
                              + " Gold Coins.");
        }

        private bool DidPlayerWin()
        {
            return playerScores[currentPlayer] == winningScore;
        }
    }

}