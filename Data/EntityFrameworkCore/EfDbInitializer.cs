using Microsoft.EntityFrameworkCore;
using QuestionAndAnswerApi.Data.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionAndAnswerApi.Data.EntityFrameworkCore
{
    public static class EfDbInitializer
    {
        public static void Initialize(EfDbContext dbContext)
        {
            dbContext.Database.Migrate();
            SeedQuesttions(dbContext);
        }

        private static void SeedQuesttions(EfDbContext dbContext)
        {
            if (dbContext.Questions.Any())
                return;

            dbContext.Questions.AddRange(new Question[]
            {
                new Question
                {
                    QuestionId=1,
                    Title="Why should I learn TypeScript?",
                    Content="TypeScript seems to be getting popular so I wondered whether it is worth my time learning it? What benefits does it give over JavaScript?",
                    UserId="1",
                    UserName="bob.test@test.com",
                    CreatedAt=DateTimeOffset.UtcNow,
                    Answers= new Answer[]
                    {
                        new Answer
                        {
                            AnswerId=1,
                            Content="To catch problems earlier speeding up your developments",
                            UserId="2",
                            UserName="jane.test@test.com",
                            CreatedAt=DateTimeOffset.UtcNow
                        },
                        new Answer
                        {
                            AnswerId=2,
                            Content="So, that you can use the JavaScript features of tomorrow, today",
                            UserId="3",
                            UserName="fred.test@test.com",
                            CreatedAt=DateTimeOffset.UtcNow
                        }
                    }
                },
                new Question
                {
                    QuestionId=2,
                    Title="Which state management tool should I use?",
                    Content="There seem to be a fair few state management tools around for React - React, Unstated, ... Which one should I use?",
                    UserId="2",
                    UserName="jane.test@test.com",
                    CreatedAt=DateTimeOffset.UtcNow,
                }
            });

            dbContext.SaveChanges();
        }

    }
}
