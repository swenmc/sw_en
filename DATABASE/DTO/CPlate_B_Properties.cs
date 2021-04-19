﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CPlate_B_Properties
    {
        private int m_ID;
        private string m_Name;
        private double m_dim1;
        private double m_dim2y;
        private double m_dim3;
        private double m_t;
        private int m_iNumberHolesAnchors;
        private int m_iNoOfAnchorsInRow;
        private int m_iNoOfAnchorsInColumn;
        private double m_a1_pos_cp_x;
        private double m_a1_pos_cp_y;
        private double m_dist_x1;
        private double m_dist_y1;
        private double m_dist_x2;
        private double m_dist_y2;
        private double m_dist_x3;
        private double m_dist_y3;
        private double m_totalDim_x;
        private double m_totalDim_y;
        private double m_area;
        private double m_volume;
        private double m_mass;
        private double m_price_PPSM_NZD;
        private double m_price_PPKG_NZD;
        private double m_price_PPP_NZD;

        public int ID
        {
            get
            {
                return m_ID;
            }

            set
            {
                m_ID = value;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }

            set
            {
                m_Name = value;
            }
        }

        public double dim1
        {
            get
            {
                return m_dim1;
            }

            set
            {
                m_dim1 = value;
            }
        }

        public double dim2y
        {
            get
            {
                return m_dim2y;
            }

            set
            {
                m_dim2y = value;
            }
        }

        public double dim3
        {
            get
            {
                return m_dim3;
            }

            set
            {
                m_dim3 = value;
            }
        }

        public double t
        {
            get
            {
                return m_t;
            }

            set
            {
                m_t = value;
            }
        }

        public int iNumberHolesAnchors
        {
            get
            {
                return m_iNumberHolesAnchors;
            }

            set
            {
                m_iNumberHolesAnchors = value;
            }
        }

        public int iNoOfAnchorsInRow
        {
            get
            {
                return m_iNoOfAnchorsInRow;
            }

            set
            {
                m_iNoOfAnchorsInRow = value;
            }
        }

        public int iNoOfAnchorsInColumn
        {
            get
            {
                return m_iNoOfAnchorsInColumn;
            }

            set
            {
                m_iNoOfAnchorsInColumn = value;
            }
        }

        public double a1_pos_cp_x
        {
            get
            {
                return m_a1_pos_cp_x;
            }

            set
            {
                m_a1_pos_cp_x = value;
            }
        }

        public double a1_pos_cp_y
        {
            get
            {
                return m_a1_pos_cp_y;
            }

            set
            {
                m_a1_pos_cp_y = value;
            }
        }

        public double dist_x1
        {
            get
            {
                return m_dist_x1;
            }

            set
            {
                m_dist_x1 = value;
            }
        }

        public double dist_y1
        {
            get
            {
                return m_dist_y1;
            }

            set
            {
                m_dist_y1 = value;
            }
        }

        public double dist_x2
        {
            get
            {
                return m_dist_x2;
            }

            set
            {
                m_dist_x2 = value;
            }
        }

        public double dist_y2
        {
            get
            {
                return m_dist_y2;
            }

            set
            {
                m_dist_y2 = value;
            }
        }

        public double dist_x3
        {
            get
            {
                return m_dist_x3;
            }

            set
            {
                m_dist_x3 = value;
            }
        }

        public double dist_y3
        {
            get
            {
                return m_dist_y3;
            }

            set
            {
                m_dist_y3 = value;
            }
        }

        public double TotalDim_x
        {
            get
            {
                return m_totalDim_x;
            }

            set
            {
                m_totalDim_x = value;
            }
        }

        public double TotalDim_y
        {
            get
            {
                return m_totalDim_y;
            }

            set
            {
                m_totalDim_y = value;
            }
        }

        public double Area
        {
            get
            {
                return m_area;
            }

            set
            {
                m_area = value;
            }
        }

        public double Volume
        {
            get
            {
                return m_volume;
            }

            set
            {
                m_volume = value;
            }
        }

        public double Mass
        {
            get
            {
                return m_mass;
            }

            set
            {
                m_mass = value;
            }
        }

        public double Price_PPSM_NZD
        {
            get
            {
                return m_price_PPSM_NZD;
            }

            set
            {
                m_price_PPSM_NZD = value;
            }
        }

        public double Price_PPKG_NZD
        {
            get
            {
                return m_price_PPKG_NZD;
            }

            set
            {
                m_price_PPKG_NZD = value;
            }
        }

        public double Price_PPP_NZD
        {
            get
            {
                return m_price_PPP_NZD;
            }

            set
            {
                m_price_PPP_NZD = value;
            }
        }

        public CPlate_B_Properties() { }
    }
}