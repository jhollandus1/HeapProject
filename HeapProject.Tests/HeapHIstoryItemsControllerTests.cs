using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HeapProject.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Localization;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace HeapProject.Tests
{
    public class HeapHIstoryItemsControllerTests
    {
        private HeapHistoryItemsController? BasicSetup()
        {
            var heapHistoryItemList = new List<HeapProject.Models.HeapHistoryItem>
            {                                                       // good enough
                new HeapProject.Models.HeapHistoryItem { Id = 1, HeapItem = "", SavedTime = DateTime.Now, HeapItemLabel = string.Empty }
            };
            var mockRepo1 = new Mock<HeapProjectContext>(new DbContextOptions<HeapProjectContext>());
            
            // problemms with only making first (storeMock) and not making any other sub-mock objects
            var storeMock = new Mock<IUserStore<ApplicationUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<IUserValidator<ApplicationUser>>(); // { new Mock<IUserValidator<ApplicationUser>>().Object };
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>>(); // { new Mock<IPasswordValidator<ApplicationUser>>().Object };
            var keyNormalizerMock = new Mock<ILookupNormalizer>();
            var errorsMock = new Mock<IdentityErrorDescriber>();
            var servicesMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<ApplicationUser>>>(); // <UserManager<ApplicationUser>>>();
            var mockRepo2 = new Mock<UserManager<ApplicationUser>>(storeMock.Object, optionsMock.Object, passwordHasherMock.Object,
                userValidators, passwordValidators, keyNormalizerMock.Object,
                errorsMock.Object, servicesMock.Object, loggerMock.Object);
            
            var mockRepo3 = new Mock<IOptions<RequestLocalizationOptions>>();
            var mockRepo4 = new Mock<IStringLocalizer<SharedResource>>();

            mockRepo1.Setup(x => x.HeapHistoryItem).ReturnsDbSet(heapHistoryItemList);

            var service = new HeapHistoryItemsController(mockRepo1.Object, mockRepo2.Object, mockRepo3.Object, mockRepo4.Object);
            
            return service;
        }


        [Fact]
        public void IsValidMaxHeap_ReturnsTrueForEmptyHeap()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test1-emptyheap.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForTooLargeHeap()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test14-too large heap.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void IsValidMaxHeap_ReturnsTrueForValidOneTerminalChildOnly()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test2-one child one terminal child.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsTrueForValidTwoChildrenOneTerminalChild()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test3-two children one terminal child.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsTrueForHeapTwoChildrenEvenlyBalanced()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test4-two children-evenly balanced.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsTrueForHeapTwoChildrenAllNodesUsed()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test5-two-children-all-nodes-used.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForHeapOneChildInvalid()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test6-one child invalid.txt");
            bool result = service.IsValidMaxHeap(fileText);
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForHeapTwoChildrenOneInvalid()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test7-two children one invalid.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForFullHeapChild8Invalid()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test8-full heap 8 invalid.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForFullHeapChild15Invalid()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test9-full heap 15 invalid.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForDuplicateItemsAtLeaves()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test10-full heap invalid due to duplicates at leaves.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForDuplicatItemsAtIntermediateNodes()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test11-full heap invalid due to duplicates at intermediate nodes.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForFieldCountsNotMatching1()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test12-invalid counts1.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidMaxHeap_ReturnsFalseForFieldCountsNotMatching2()
        {
            // Arrange
            HeapHistoryItemsController? service = BasicSetup();

            // Act
            string fileText = System.IO.File.ReadAllText("..\\..\\..\\..\\HeapProject.Tests\\test-files\\test13-invalid counts2.txt");
            bool result = service.IsValidMaxHeap(fileText);

            // Assert
            Assert.False(result);
        }
         
    }
}
