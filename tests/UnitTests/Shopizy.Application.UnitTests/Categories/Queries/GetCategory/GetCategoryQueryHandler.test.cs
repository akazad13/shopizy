// namespace Shopizy.Application.UnitTests.Categories.Queries.GetCategory;

// public class GetCategoryQueryHandlerTests
// {
//     private readonly GetCategoryQueryHandler _handler;
//     private readonly Mock<ICategoryRepository> _mockCategoryRepository;

//     public GetCategoryQueryHandlerTests()
//     {
//         _mockCategoryRepository = new Mock<ICategoryRepository>();
//         _handler = new GetCategoryQueryHandler(_mockCategoryRepository.Object);
//     }

//     [Fact]
//     public async Task GetCategory_WhenCategoryIsFound_ReturnCategoryDetails()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery();
//         _mockCategoryRepository
//             .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(query.CategoryId)))
//             .ReturnsAsync(Category.Create(Constants.Category.Name, Constants.Category.ParentId));

//         // Act
//         var result = await _handler.Handle(query, default);

//         // Assert
//         result.IsError.Should().BeFalse();
//         result.Value.Should().NotBeNull();
//         result.Value.Id.Should().Be(query.CategoryId);
//         result.Value.Name.Should().Be(Constants.Category.Name);
//         result.Value.ParentId.Should().Be(Constants.Category.ParentId);
//     }

//     [Fact]
//     public async Task GetCategory_WhenCategoryIdDoesNotExist_ThrowsException()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery();
//         _mockCategoryRepository
//             .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(query.CategoryId)))
//             .ReturnsAsync((Category)null);

//         // Act & Assert
//         await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, default));
//     }

//     [Fact]
//     public async Task GetCategory_WhenParentCategoryIdIsProvided_ReturnAllSubcategories()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery(parentId: "ParentCategoryId");
//         _mockCategoryRepository
//             .Setup(c => c.GetSubcategoriesByParentIdAsync(CategoryId.Create(query.ParentId)))
//             .ReturnsAsync(
//                 new List<Category>
//                 {
//                     Category.Create("Subcategory1", query.ParentId),
//                     Category.Create("Subcategory2", query.ParentId),
//                 }
//             );

//         // Act
//         var result = await _handler.Handle(query, default);

//         // Assert
//         result.IsError.Should().BeFalse();
//         result.Value.Count.Should().Be(2);
//         result.Value[0].Name.Should().Be("Subcategory1");
//         result.Value[1].Name.Should().Be("Subcategory2");
//     }

//     [Fact]
//     public async Task GetCategory_WhenNoSubcategoriesAreFound_ReturnEmptyList()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery(parentId: "ParentCategoryId");
//         _mockCategoryRepository
//             .Setup(c => c.GetSubcategoriesByParentIdAsync(CategoryId.Create(query.ParentId)))
//             .ReturnsAsync(new List<Category>());

//         // Act
//         var result = await _handler.Handle(query, default);

//         // Assert
//         result.IsError.Should().BeFalse();
//         result.Value.Count.Should().Be(0);
//     }

//     [Fact]
//     public async Task GetCategory_WhenConcurrentRequestsForSameCategoryId_DoesNotBlock()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery();
//         _mockCategoryRepository
//             .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(query.CategoryId)))
//             .ReturnsAsync(Category.Create(Constants.Category.Name, Constants.Category.ParentId));

//         var tasks = Enumerable.Range(1, 10).Select(_ => _handler.Handle(query, default)).ToList();

//         // Act
//         await Task.WhenAll(tasks);

//         // Assert
//         // Ensure all tasks completed without any exceptions
//         tasks.ForEach(t => t.Result.IsError.Should().BeFalse());
//     }

//     [Fact]
//     public async Task GetCategory_WhenCategoryNameExceedsMaxLength_ThrowsException()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery();
//         var longName = new string('a', CategoryConstants.MaxNameLength + 1);
//         _mockCategoryRepository
//             .Setup(c => c.GetCategoryByIdAsync(CategoryId.Create(query.CategoryId)))
//             .ReturnsAsync(Category.Create(longName, Constants.Category.ParentId));

//         // Act & Assert
//         await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, default));
//     }

//     [Fact]
//     public async Task GetCategory_WhenPaginationParametersAreProvided_ReturnPaginatedCategories()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery(pageNumber: 1, pageSize: 10);
//         _mockCategoryRepository
//             .Setup(c => c.GetCategoriesAsync(It.IsAny<int>(), It.IsAny<int>()))
//             .ReturnsAsync(
//                 new PaginatedList<Category>(
//                     new List<Category>
//                     {
//                         Category.Create("Category1", null),
//                         Category.Create("Category2", null),
//                     },
//                     2,
//                     10,
//                     1,
//                     1
//                 )
//             );

//         // Act
//         var result = await _handler.Handle(query, default);

//         // Assert
//         result.IsError.Should().BeFalse();
//         result.Value.Items.Count.Should().Be(2);
//         result.Value.Items[0].Name.Should().Be("Category1");
//         result.Value.Items[1].Name.Should().Be("Category2");
//         result.Value.PageNumber.Should().Be(1);
//         result.Value.PageSize.Should().Be(10);
//         result.Value.TotalCount.Should().Be(2);
//         result.Value.TotalPages.Should().Be(1);
//     }

//     [Fact]
//     public async Task GetCategory_WhenNoCategoriesAreFoundForSearchTerm_ReturnEmptyList()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery(searchTerm: "NonExistentTerm");
//         _mockCategoryRepository
//             .Setup(c => c.GetCategoriesBySearchTermAsync(query.SearchTerm))
//             .ReturnsAsync(new List<Category>());

//         // Act
//         var result = await _handler.Handle(query, default);

//         // Assert
//         result.IsError.Should().BeFalse();
//         result.Value.Count.Should().Be(0);
//     }

//     [Fact]
//     public async Task GetCategory_WhenCategoryNameSearchIsCaseInsensitive_ReturnMatchingCategories()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery(searchTerm: "category1");
//         var category1 = Category.Create("Category1", null);
//         var category2 = Category.Create("category2", null);
//         _mockCategoryRepository
//             .Setup(c =>
//                 c.GetCategoriesBySearchTermAsync(
//                     It.Is<string>(s => s.ToLower() == query.SearchTerm.ToLower())
//                 )
//             )
//             .ReturnsAsync(new List<Category> { category1, category2 });

//         // Act
//         var result = await _handler.Handle(query, default);

//         // Assert
//         result.IsError.Should().BeFalse();
//         result.Value.Count.Should().Be(2);
//         result.Value.Should().Contain(c => c.Name == "Category1");
//         result.Value.Should().Contain(c => c.Name == "category2");
//     }

//     [Fact]
//     public async Task GetCategory_WhenSortByCreationDateRequested_ReturnSortedListOfCategories()
//     {
//         // Arrange
//         var query = GetCategoryQueryUtils.CreateQuery(sortBy: "creationDate");
//         var category1 = Category.Create(
//             Constants.Category.Name,
//             Constants.Category.ParentId,
//             DateTime.Now.AddDays(-1)
//         );
//         var category2 = Category.Create("Category2", Constants.Category.ParentId, DateTime.Now);
//         _mockCategoryRepository
//             .Setup(c =>
//                 c.GetCategoriesAsync(
//                     It.IsAny<int>(),
//                     It.IsAny<int>(),
//                     It.Is<string>(s => s == query.SortBy)
//                 )
//             )
//             .ReturnsAsync(
//                 new PaginatedList<Category>(
//                     new List<Category> { category1, category2 },
//                     2,
//                     10,
//                     1,
//                     1
//                 )
//             );

//         // Act
//         var result = await _handler.Handle(query, default);

//         // Assert
//         result.IsError.Should().BeFalse();
//         result.Value.Items[0].Name.Should().Be(category2.Name);
//         result.Value.Items[1].Name.Should().Be(category1.Name);
//     }
// }
