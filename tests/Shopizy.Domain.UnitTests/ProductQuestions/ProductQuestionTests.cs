using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.ProductQuestions.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.ProductQuestions;

public class ProductQuestionTests
{
    [Fact]
    public void Create_ShouldInitializeQuestion()
    {
        var productId = ProductId.CreateUnique();
        var userId = UserId.CreateUnique();
        var questionText = "Does this come with a warranty?";

        var question = ProductQuestion.Create(productId, userId, questionText);

        question.ShouldNotBeNull();
        question.ProductId.ShouldBe(productId);
        question.AskedByUserId.ShouldBe(userId);
        question.Question.ShouldBe(questionText);
        question.IsAnswered.ShouldBeFalse();
        question.Answer.ShouldBeNull();
    }

    [Fact]
    public void AddAnswer_WhenNotAnswered_ShouldAddAnswer()
    {
        var question = ProductQuestion.Create(
            ProductId.CreateUnique(),
            UserId.CreateUnique(),
            "Warranty info?"
        );
        var adminId = UserId.CreateUnique();

        var result = question.AddAnswer(adminId, "Yes, 1 year warranty.");

        result.IsError.ShouldBeFalse();
        question.IsAnswered.ShouldBeTrue();
        question.Answer.ShouldNotBeNull();
        question.Answer.AnsweredByUserId.ShouldBe(adminId);
        question.Answer.Answer.ShouldBe("Yes, 1 year warranty.");
    }

    [Fact]
    public void AddAnswer_WhenAlreadyAnswered_ShouldReturnError()
    {
        var question = ProductQuestion.Create(
            ProductId.CreateUnique(),
            UserId.CreateUnique(),
            "Warranty info?"
        );
        var adminId = UserId.CreateUnique();
        question.AddAnswer(adminId, "Yes, 1 year.");

        var secondResult = question.AddAnswer(adminId, "Updated answer");

        secondResult.IsError.ShouldBeTrue();
    }

    [Fact]
    public void ValueObjects_CreateUniqueAndCreate_ShouldInitialize()
    {
        var raw1 = Guid.NewGuid();
        var qId1 = ProductQuestionId.CreateUnique();
        var qId2 = ProductQuestionId.Create(raw1);
        var qId3 = ProductQuestionId.Create(raw1);

        var raw2 = Guid.NewGuid();
        var aId1 = ProductAnswerId.CreateUnique();
        var aId2 = ProductAnswerId.Create(raw2);
        var aId3 = ProductAnswerId.Create(raw2);

        qId1.Value.ShouldNotBe(Guid.Empty);
        qId2.Value.ShouldNotBe(Guid.Empty);
        qId2.ShouldBe(qId3);
        qId2.GetHashCode().ShouldBe(qId3.GetHashCode());

        aId1.Value.ShouldNotBe(Guid.Empty);
        aId2.Value.ShouldNotBe(Guid.Empty);
        aId2.ShouldBe(aId3);
        aId2.GetHashCode().ShouldBe(aId3.GetHashCode());
    }
}
