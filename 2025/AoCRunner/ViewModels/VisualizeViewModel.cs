using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Helpers.Drawing;

namespace AoCRunner.ViewModels
{
    public class VisualizeViewModel : ViewModelBase
    {
        public ObservableCollection<DrawableCoordinate> CoordinatesToDraw { get; }

        public VisualizeViewModel(DrawableCoordinate[] coordinates)
        {
            // This doesn't work but I'm tired of debugging this shitty UI library at the moment
            var maxY = coordinates.Max(x => x.Y);

            CoordinatesToDraw = new(coordinates.Select(x => new DrawableCoordinate
            {
                // Convert to a 15px grid
                Color = x.Color,
                Text = x.Text,
                X = x.X * 15,
                Y = (maxY - x.Y) * 15
            }));
        }
    }
}
