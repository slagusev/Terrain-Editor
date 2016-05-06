﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using MoreLinq;
using TerrainEditor.Utilities;

namespace TerrainEditor.ViewModels
{
    public class DynamicMesh  : ViewModelBase
    {
        private bool m_isClosed = true;
        private bool m_splitWhenDifferent = false;

        private int m_smoothFactor = 5;
        private double m_strechThreshold  = 0.5;
        private int m_splitCornersThreshold = 90;
        private int m_pixelsPerUnit = 96; 

        private FillMode m_fillMode = FillMode.None;
        private Color m_ambientColor = Colors.White;
        private Point3D m_position = new Point3D(0,0,1);
        private string m_name = "New Terrain";

        private UvMapping m_uvMapping;

        private Model3DGroup m_meshCache;
        private bool m_isDirty = true;

        public bool IsClosed
        {
            get { return m_isClosed; }
            set
            {
                if (value == m_isClosed) return;
                m_isClosed = value;
                OnPropertyChanged();
            }
        }
        public int SmoothFactor
        {
            get { return m_smoothFactor; }
            set
            {
                if (value == m_smoothFactor) return;
                m_smoothFactor = value;
                OnPropertyChanged();
            }
        }
        public Color AmbientColor
        {
            get { return m_ambientColor; }
            set
            {
                if (value.Equals(m_ambientColor)) return;
                m_ambientColor = value;
                OnPropertyChanged();
            }
        }
        public double StrechThreshold
        {
            get { return m_strechThreshold; }
            set
            {
                if (value.Equals(m_strechThreshold)) return;
                m_strechThreshold = value;
                OnPropertyChanged();
            }
        }
        public bool SplitWhenDifferent
        {
            get { return m_splitWhenDifferent; }
            set
            {
                if (value == m_splitWhenDifferent) return;
                m_splitWhenDifferent = value;
                OnPropertyChanged();
            }
        }
        public int SplitCornersThreshold
        {
            get { return m_splitCornersThreshold; }
            set
            {
                if (value == m_splitCornersThreshold) return;
                m_splitCornersThreshold = value;
                OnPropertyChanged();
            }
        }
        public int PixelsPerUnit
        {
            get { return m_pixelsPerUnit; }
            set
            {
                if (value == m_pixelsPerUnit) return;
                m_pixelsPerUnit = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return m_name; }
            set
            {
                if (value == m_name) return;
                m_name = value;
                OnPropertyChanged();
            }
        }
        public Point3D Position
        {
            get { return m_position; }
            set
            {
                if (value.Equals(m_position)) return;
                m_position = value;
                OnPropertyChanged();
            }
        }

        public FillMode FillMode
        {
            get { return m_fillMode; }
            set
            {
                if (value == m_fillMode) return;
                m_fillMode = value;
                OnPropertyChanged();
            }
        }
        public UvMapping UvMapping
        {
            get { return m_uvMapping; }
            set
            {
                if (Equals(value, m_uvMapping)) return;
                m_uvMapping = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<VertexInfo> Vertices { get; }

        public DynamicMesh(IEnumerable<VertexInfo> vertices = null)
        {
            vertices = vertices ?? Enumerable.Empty<VertexInfo>();
            Vertices = new ObservableCollection<VertexInfo>(vertices);

            PropertyChanged += (sender, args) => m_isDirty = true;
            Vertices.CollectionChanged += VerticesOnCollectionChanged;

            VerticesOnCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,Vertices));
        }

        private void VerticesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    args.NewItems.Cast<VertexInfo>().ForEach(info => info.PropertyChanged += VertexChanged);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    args.OldItems.Cast<VertexInfo>().ForEach(info => info.PropertyChanged -= VertexChanged);
                    break;
                default:
                    throw new NotImplementedException();
            }
            OnPropertyChanged(nameof(Vertices));
        }
        private void VertexChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(nameof(Vertices));
        }

        public Model3DGroup Mesh
        {
            get
            {
                if (m_isDirty)
                {
                    m_meshCache = DynamicMeshBuilder.GenerateMesh(this);
                    m_meshCache.Transform = new TranslateTransform3D(Position.X,Position.Y,Position.Z);
                    m_meshCache.SetName(Name);
                    m_isDirty = false;
                }

                return m_meshCache;
            }
        }
}
}