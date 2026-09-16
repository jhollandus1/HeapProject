using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HeapProject.Tests
{
    public class HeapHIstoryItemsControllerTests
    {
        private HeapHistoryItemsController? BasicSetup()
        {
            var heapHistoryItemList = new List<HeapProject.Models.HeapHistoryItem>
            {                                                       // good enough
                new HeapProject.Models.HeapHistoryItem { Id = 1, HeapItem = "", SavedTime = DateTime.Now }
            };
            var mockRepo = new Mock<HeapProjectContext>(new DbContextOptions<HeapProjectContext>());

            // Need to do: Moqing the DbContextOptions? Moqing 
            mockRepo.Setup(x => x.HeapHistoryItem).ReturnsDbSet(heapHistoryItemList);

            var service = new HeapHistoryItemsController(mockRepo.Object);
            
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
