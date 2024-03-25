using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Products.Commands.CreateProduct;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Shopizy.Application.UnitTests.Products.Commands.TestUtils;

public static class CreateProductCommandUtils
{
    public static CreateProductCommand CreateCommand()
    {
        var fileMock = new Mock<IFormFile>();

        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        ms.Dispose();

        return new CreateProductCommand(
            Constants.User.Id.Value,
            Constants.Product.Name,
            Constants.Product.Description,
            Constants.Category.Id.Value,
            Constants.Product.UnitPrice,
            Constants.Product.Currency,
            Constants.Product.Discount,
            Constants.Product.Sku,
            Constants.Product.Brand,
            Constants.Product.Tags,
            Constants.Product.Barcode,
            Constants.Product.StockQuantity,
            [fileMock.Object, fileMock.Object],
            []
        );
    }
}
