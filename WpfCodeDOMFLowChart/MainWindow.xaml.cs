using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
namespace WpfCodeDOMFLowChart
{
    public partial class MainWindow : Window
    {
        private List<UIElement> nodes = new List<UIElement>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            var node = new Rectangle
            {
                Width = 80,
                Height = 30,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };
            var textBox = new TextBox
            {
                Width = 80,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            var nodeContainer = new Grid();
            nodeContainer.Children.Add(node);
            nodeContainer.Children.Add(textBox);
            Canvas.SetLeft(nodeContainer, e.GetPosition(mainCanvas).X - node.Width / 2);
            Canvas.SetTop(nodeContainer, e.GetPosition(mainCanvas).Y - node.Height / 2);
            mainCanvas.Children.Add(nodeContainer);
            textBox.Focus();
            nodes.Add(nodeContainer);
            if (nodes.Count > 1)
            {
                var newNode = nodes.Last();
                var nearestNode = FindNearestUpperNode(newNode);

                if (nearestNode != null)
                {
                    DrawLineBetweenNodes(newNode, nearestNode);
                }
            }
        }
        private UIElement FindNearestUpperNode(UIElement newNode)
        {
            var upperNodes = nodes
            .Where(node => Canvas.GetTop(node) < Canvas.GetTop(newNode) && node != newNode)
            .OrderBy(node => Canvas.GetTop(node));

            return upperNodes.LastOrDefault();
        }
        private void DrawLineBetweenNodes(UIElement startNode, UIElement endNode)
        {
            var startPoint = new Point(Canvas.GetLeft(startNode) + startNode.RenderSize.Width / 2, Canvas.GetTop(startNode) + startNode.RenderSize.Height);
            var endPoint = new Point(Canvas.GetLeft(endNode) + endNode.RenderSize.Width / 2, Canvas.GetTop(endNode) + endNode.RenderSize.Height);
            var line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Tag = new Tuple<UIElement, UIElement>(startNode, endNode),
            };

            mainCanvas.Children.Add(line);
        }
        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (nodes.Count > 0)
            {
                var lastNode = nodes.Last();
                RemoveLinesConnectedToNode(lastNode);
                mainCanvas.Children.Remove(lastNode);
                nodes.Remove(lastNode);
            }
        }
        private void RemoveLinesConnectedToNode(UIElement node)
        {
            var linesToRemove = new List<UIElement>();
            foreach (var line in mainCanvas.Children)
            {
                if (line is Line lineElement)
                {
                    var tags = lineElement.Tag as Tuple<UIElement, UIElement>;
                    if (tags != null && (tags.Item1 == node || tags.Item2 == node))
                    {
                        linesToRemove.Add(lineElement);
                    }
                }
            }
            foreach (var line in linesToRemove)
            {
                mainCanvas.Children.Remove(line);
            }
        }
    }
}
