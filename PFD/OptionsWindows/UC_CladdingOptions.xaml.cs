﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BaseClasses;
using MATH;
using PFD.Infrastructure;

namespace PFD
{
    public partial class UC_CladdingOptions : UserControl
    {
        private CPFDViewModel _pfdVM;
        private bool CladdingOptionsChanged = false;
        private bool ErrorDetected = false;

        //private bool RecreateModelRequired = false;
        public UC_CladdingOptions(CPFDViewModel pfdVM)
        {
            InitializeComponent();

            _pfdVM = pfdVM;

            CladdingOptionsChanged = false;
            
            pfdVM._claddingOptionsVM.PropertyChanged += HandleCladdingOptionsPropertyChangedEvent;

            this.DataContext = pfdVM._claddingOptionsVM;
        }

        private void HandleCladdingOptionsPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            if (sender is CladdingOptionsViewModel)
            {
                CladdingOptionsChanged = true;
            }
            if (sender is FibreglassProperties)
            {
                //check for collisions
                //ak sa pri zmene najde kolizia, tak sa snazime aktualne editovany objekt vratit na povodne hodnoty
                //To Mato - tu uz nebudeme uvazovat tie model options nie???

                FibreglassProperties f = sender as FibreglassProperties;

                if (_pfdVM._claddingOptionsVM.CollisionsExists())
                {
                    ErrorDetected = true;
                    MessageBox.Show(_pfdVM.PFDMainWindow, "Collision detected.");

                    if (e.PropertyName == "X") f.UndoX();
                    else if (e.PropertyName == "Y") f.UndoY();
                    else if (e.PropertyName == "Side") f.UndoSide();
                    else if (e.PropertyName == "Length") f.UndoLength();
                }
                //748 - toto treba zapnut ak bude validacia v poriadku
                else if (!f.ValidateMaxHeight()) //ak sa nezmesti
                {
                    ErrorDetected = true;
                    MessageBox.Show("Fibreglass is outside of building dimensions.");

                    if (e.PropertyName == "X") f.UndoX();
                    else if (e.PropertyName == "Y") f.UndoY();
                    else if (e.PropertyName == "Side") f.UndoSide();
                    else if (e.PropertyName == "Length") f.UndoLength();
                }
                else if (CDoorsAndWindowsHelper.IsFibreglassInCollisionWithAnyDoors(_pfdVM, f))
                {
                    ErrorDetected = true;
                    MessageBox.Show("Fibreglass is in collison with doors.");

                    if (e.PropertyName == "X") f.UndoX();
                    else if (e.PropertyName == "Y") f.UndoY();
                    else if (e.PropertyName == "Side") f.UndoSide();
                    else if (e.PropertyName == "Length") f.UndoLength();
                }
                else if (CDoorsAndWindowsHelper.IsFibreglassInCollisionWithAnyWindow(_pfdVM, f))
                {
                    ErrorDetected = true;
                    MessageBox.Show("Fibreglass is in collison with window.");

                    if (e.PropertyName == "X") f.UndoX();
                    else if (e.PropertyName == "Y") f.UndoY();
                    else if (e.PropertyName == "Side") f.UndoSide();
                    else if (e.PropertyName == "Length") f.UndoLength();
                }
                ErrorDetected = false;
                CladdingOptionsChanged = true;
            }
        }

        
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //nefungovalo akoby neumoznit opustit tab, tak sa naspat selectne Cladding tab ak je chyba (753)
            if (ErrorDetected) { e.Handled = true; _pfdVM.PFDMainWindow.SelectCladdingTab(); return; } //neumoznime opustit tab ak je error

            if (CladdingOptionsChanged)
            {
                _pfdVM.CladdingOptionsChanged = true;
            }
            CladdingOptionsChanged = false;
        }

        private void BtnFiberglassGenerator_Click(object sender, RoutedEventArgs e)
        {
            FibreglassGeneratorWindow w = new FibreglassGeneratorWindow(_pfdVM);
            w.ShowDialog();

            FibreglassGeneratorViewModel generatorViewModel = w.DataContext as FibreglassGeneratorViewModel;
            if (generatorViewModel == null) return;
            if (generatorViewModel.AddFibreglass)
            {
                List<FibreglassProperties> mergedLists = new List<FibreglassProperties>();
                List<FibreglassProperties> itemsToAdd = w.GetFibreglassPropertiesWithNoCollisions();
                bool collisionDetected = false;
                if (_pfdVM._modelOptionsVM.CollisionInsertNewOne) mergedLists = MergeLists_InsertOnlyItemsWhereAvailable(itemsToAdd, _pfdVM._claddingOptionsVM.FibreglassProperties.ToList(), out collisionDetected);
                else mergedLists = MergeLists_DeleteOriginal(itemsToAdd, _pfdVM._claddingOptionsVM.FibreglassProperties.ToList(), out collisionDetected);

                if (collisionDetected)
                {
                    MessageBoxResult res = MessageBox.Show("Collisions were detected. Do you want to solve them automatically?", "Attention", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.No) return; //do not add, do not do anything, do not add new generated items
                }

                List<FibreglassProperties> collisions = CDoorsAndWindowsHelper.GetCollisionsWithDoorsOrWindows(_pfdVM, mergedLists);
                if (collisions.Count > 0)
                {
                    MessageBox.Show($"We found {collisions.Count} collisions with doors or windows. Fibreglass in collision will be removed.", "Attention");
                    foreach (FibreglassProperties collision_fp in collisions) mergedLists.Remove(collision_fp);                    
                }

                _pfdVM._claddingOptionsVM.FibreglassProperties = new ObservableCollection<FibreglassProperties>(mergedLists);
            }
            else if (generatorViewModel.DeleteFibreglass)
            {
                List<FibreglassProperties> fibreglassToDelete = w.GetFibreglassToDelete();
                if (fibreglassToDelete.Count == 0) return;

                List<FibreglassProperties> fibreglassProperties = new List<FibreglassProperties>();

                foreach (FibreglassProperties fp in _pfdVM._claddingOptionsVM.FibreglassProperties)
                {
                    //bool isTheItemToDelete = fibreglassToDelete.Exists(f => MathF.d_equal(f.X, fp.X) && MathF.d_equal(f.Y, fp.Y) && MathF.d_equal(f.Length, fp.Length) && f.Side == fp.Side);
                    //upravujem na vymazanie z rovnakej Side
                    bool isTheItemToDelete = fibreglassToDelete.Exists(f => f.Side == fp.Side);
                    if (!isTheItemToDelete) fibreglassProperties.Add(fp);
                }
                _pfdVM._claddingOptionsVM.FibreglassProperties = new ObservableCollection<FibreglassProperties>(fibreglassProperties);
            }
        }


        private List<FibreglassProperties> MergeLists_InsertOnlyItemsWhereAvailable(IList<FibreglassProperties> newItems, IList<FibreglassProperties> sourceItems, out bool collisionDetected)
        {
            collisionDetected = false;

            List<FibreglassProperties> mergedLists = new List<FibreglassProperties>(sourceItems);            

            foreach (FibreglassProperties new_f in newItems)
            {
                bool existsSameItem = mergedLists.Exists(f => f.Equals(new_f));
                if (existsSameItem) { collisionDetected = true; continue; } 

                if (!mergedLists.Exists(f => f.IsInCollisionWith(new_f))) mergedLists.Add(new_f); //no collision - ADD
                else collisionDetected = true;
            }

            return mergedLists;
        }

        private List<FibreglassProperties> MergeLists_DeleteOriginal(IList<FibreglassProperties> newItems, IList<FibreglassProperties> sourceItems, out bool collisionDetected)
        {
            collisionDetected = false;

            List<FibreglassProperties> mergedLists = new List<FibreglassProperties>(newItems);

            foreach (FibreglassProperties original_f in sourceItems)
            {
                bool existsSameItem = mergedLists.Exists(f => f.Equals(original_f));
                if (existsSameItem) { collisionDetected = true; continue; }

                if (!mergedLists.Exists(f => f.IsInCollisionWith(original_f))) mergedLists.Add(original_f); //no collision - ADD
                else collisionDetected = true;
            }

            return mergedLists;
        }

        private void BtnApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            if (ErrorDetected) { return; }

            if (CladdingOptionsChanged)
            {
                _pfdVM.CladdingOptionsChanged = true;
            }
            CladdingOptionsChanged = false;
        }
    }
}
