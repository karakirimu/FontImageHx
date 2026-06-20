using FontImageHx;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FontImageHxTest
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class MainViewModelUnit
    {
        [Test]
        public void Close_UsesApplicationService()
        {
            Mock<IApplicationService> applicationServiceMock = new();
            MainViewModel viewModel = CreateViewModel(applicationServiceMock: applicationServiceMock);

            viewModel.Close.Execute(null);

            applicationServiceMock.Verify(static service => service.Shutdown(), Times.Once);
        }

        [Test]
        public void NewProfile_UpdatesImagesAndEditingStateWhenDialogReturnsImages()
        {
            List<ImageProperty> images =
            [
                new() { Character = 'A' },
                new() { Character = 'B', NewLine = true }
            ];
            Mock<IMainDialogService> dialogServiceMock = new();
            dialogServiceMock
                .Setup(static service => service.ShowNewProfileDialog())
                .Returns(images);
            MainViewModel viewModel = CreateViewModel(dialogServiceMock: dialogServiceMock);

            viewModel.NewProfile.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.ConvertedImages.Select(static image => image.Character), Is.EqualTo(new[] { 'A', 'B' }));
                Assert.That(viewModel.IsEditing, Is.True);
                Assert.That(viewModel.WindowTitle, Is.EqualTo("Untitled - FontImageHx"));
            });
        }

        [Test]
        public void OpenProfile_RestoresImagesAndTextFromFileService()
        {
            List<ImageProperty> images =
            [
                new()
                {
                    Character = 'A',
                    CharWidth = "2",
                    CharHeight = "1",
                    HexHorizontal = "0x80"
                },
                new()
                {
                    Character = 'B',
                    NewLine = true,
                    CharWidth = "2",
                    CharHeight = "1",
                    HexHorizontal = "0x40"
                }
            ];
            Mock<IMainFileService> fileServiceMock = new();
            fileServiceMock
                .Setup(static service => service.OpenProfile())
                .Returns(("sample.fip", images));
            MainViewModel viewModel = CreateViewModel(fileServiceMock: fileServiceMock);

            viewModel.OpenProfile.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.ConvertedImages.Count, Is.EqualTo(2));
                Assert.That(viewModel.TextAreaString, Is.EqualTo("A\nB"));
                Assert.That(viewModel.WindowTitle, Is.EqualTo("sample.fip - FontImageHx"));
                Assert.That(viewModel.IsEditing, Is.True);
                Assert.That(viewModel.ConvertedImages[0].ViewSource.Width, Is.EqualTo(2));
            });
        }

        [Test]
        public void ClickRight_MovesEditBitmapWithoutTouchingWpfControls()
        {
            MainViewModel viewModel = CreateViewModel();
            viewModel.LastSelectedImage = CreateImageProperty("2", "2", "0x80,0x00");

            viewModel.ClickRight.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.EditBitmap, Is.Not.Null);
                Assert.That(viewModel.EditBitmap![0], Is.EqualTo(new byte[] { 0, 1 }));
                Assert.That(viewModel.EditBitmap[1], Is.EqualTo(new byte[] { 0, 0 }));
            });
        }

        [Test]
        public void ImageUpdate_UsesEditBitmapAsSourceOfTruth()
        {
            MainViewModel viewModel = CreateViewModel();
            ImageProperty image = CreateImageProperty("2", "2", "0x80,0x00");
            viewModel.ConvertedImages.Add(image);
            viewModel.LastSelectedIndex = 0;
            viewModel.LastSelectedImage = image;
            viewModel.UpdateEditBitmapCell(1, 1, true);

            viewModel.ImageUpdate.Execute(null);

            Assert.Multiple(() =>
            {
                Assert.That(viewModel.ConvertedImages[0].HexHorizontal, Is.EqualTo("0x80,0x40"));
                Assert.That(viewModel.LastSelectedImage.HexVertical, Is.EqualTo("0x80,0x40"));
                Assert.That(viewModel.EditBitmap, Is.Not.Null);
                Assert.That(viewModel.EditBitmap![1][1], Is.EqualTo(1));
            });
        }

        [Test]
        public void ImageUpdate_DisposesPreviousImageWhenReplacingSelectedEntry()
        {
            MainViewModel viewModel = CreateViewModel();
            ImageProperty image = CreateImageProperty("2", "2", "0x80,0x00");
            viewModel.ConvertedImages.Add(image);
            viewModel.LastSelectedIndex = 0;
            viewModel.LastSelectedImage = image;
            viewModel.UpdateEditBitmapCell(1, 1, true);

            viewModel.ImageUpdate.Execute(null);

            Assert.Throws<ObjectDisposedException>(() => image.ViewSource.GetPixel(0, 0));
        }

        private static MainViewModel CreateViewModel(
            Mock<IApplicationService>? applicationServiceMock = null,
            Mock<IMainDialogService>? dialogServiceMock = null,
            Mock<IMainFileService>? fileServiceMock = null)
        {
            return new MainViewModel(
                (applicationServiceMock ?? new Mock<IApplicationService>()).Object,
                (dialogServiceMock ?? new Mock<IMainDialogService>()).Object,
                (fileServiceMock ?? new Mock<IMainFileService>()).Object);
        }

        private static ImageProperty CreateImageProperty(string charWidth, string charHeight, string hexHorizontal)
        {
            ImageProperty image = new()
            {
                Character = 'A',
                CharWidth = charWidth,
                CharHeight = charHeight,
                HexHorizontal = hexHorizontal,
                HexVertical = hexHorizontal
            };
            MainImageLogic.RestoreViewSource(image);
            return image;
        }
    }
}
