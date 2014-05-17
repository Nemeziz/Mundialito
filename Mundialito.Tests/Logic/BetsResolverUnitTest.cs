﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Mundialito.DAL.Accounts;
using Mundialito.DAL.Bets;
using Mundialito.DAL.Games;
using Mundialito.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mundialito.Tests.Logic
{
    [TestClass]
    public class BetsResolverUnitTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUnResolvedGame()
        {

            var betsRepository = new Mock<IBetsRepository>();
            var resolver = new BetsResolver(betsRepository.Object);
            resolver.ResolveBets(new Game()
            {
                GameId = 1,
                Date = new DateTime().AddDays(2)
            });


        }

        [TestMethod]
        public void TestBetResolving()
        {
            var betsRepository = new Mock<IBetsRepository>();
            betsRepository.Setup(res => res.GetGameBets(It.IsAny<int>())).Returns(new List<Bet> { 
                new Bet() { 
                    BetId = 1, User = new MundialitoUser() { Id = "1" } , Game = new Game() { GameId = 1, Date = DateTime.Now.ToUniversalTime()}, HomeScore = 1, AwayScore = 1 , CardsMark = "X", CornersMark = "1"
                },
                new Bet() { 
                    BetId = 2, User = new MundialitoUser() { Id = "2" } , Game = new Game() { GameId = 1, Date = DateTime.Now.ToUniversalTime()}, HomeScore = 2, AwayScore = 1 , CardsMark = "2", CornersMark = "1"
                },
                new Bet() { 
                    BetId = 3, User = new MundialitoUser() { Id = "3" } , Game = new Game() { GameId = 1, Date = DateTime.Now.ToUniversalTime()}, HomeScore = 2, AwayScore = 2 , CardsMark = "1", CornersMark = "X"
                },
                new Bet() { 
                    BetId = 4, User = new MundialitoUser() { Id = "1" } , Game = new Game() { GameId = 2, Date = DateTime.Now.ToUniversalTime()}, HomeScore = 2, AwayScore = 2 , CardsMark = "X", CornersMark = "2"
                }
            });
            var resolver = new BetsResolver(betsRepository.Object);
            resolver.ResolveBets(new Game()
            {
                GameId = 1,
                Date = DateTime.Now.ToUniversalTime(),
                HomeScore = 1,
                AwayScore = 1,
                CardsMark = "X",
                CornersMark = "1"
            });
            betsRepository.Verify(res => res.UpdateBet(It.Is<Bet>(bet => bet.BetId == 1 && bet.Points == 7)));
            betsRepository.Verify(res => res.UpdateBet(It.Is<Bet>(bet => bet.BetId == 2 && bet.Points == 1)));
            betsRepository.Verify(res => res.UpdateBet(It.Is<Bet>(bet => bet.BetId == 3 && bet.Points == 3)));
        }
    }
}

