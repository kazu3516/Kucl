using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Kucl {

    #region YUV Color
    /// <summary>
    /// 0�`255�ɃX�P�[�����O���ꂽYUV�v�f������Color�\����
    /// </summary>
    public struct YUVColor {

        #region �����o�ϐ�
        private int m_Y;
        private int m_U;
        private int m_V;
        #endregion

        #region �R���X�g���N�^
        /// <summary>
        /// Y,U,V�̒l���w�肵��YUVColor�\���̂����������܂��B
        /// </summary>
        /// <param name="y"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public YUVColor(int y,int u,int v) {
            this.m_Y = y;
            this.m_U = u;
            this.m_V = v;
        }
        /// <summary>
        /// �w�肵��RGB Color�ɑΉ�����YUVColor�\���̂����������܂��B
        /// </summary>
        /// <param name="c"></param>
        public YUVColor(Color c) {
            this.m_Y = 0;
            this.m_U = 0;
            this.m_V = 0;
            this.m_Y = this.GetY(c.R,c.G,c.B);
            this.m_U = this.GetU(c.R,c.G,c.B);
            this.m_V = this.GetV(c.R,c.G,c.B);
        }
        #endregion

        #region �v���p�e�B
        /// <summary>
        /// Y�̒l���擾���܂��B
        /// </summary>
        public int Y {
            get {
                return this.m_Y;
            }
        }
        /// <summary>
        /// U�̒l���擾���܂��B
        /// </summary>
        public int U {
            get {
                return this.m_U;
            }
        }
        /// <summary>
        /// V�̒l���擾���܂��B
        /// </summary>
        public int V {
            get {
                return this.m_V;
            }
        }
        #endregion

        #region Get���\�b�h

        #region RGB to YUV
        private int GetY(int r,int g,int b) {
            int y = (int)(0.299 * r + 0.587 * g + 0.114 * b);
            y = this.Normalize(y,0,255);
            return y;
        }
        private int GetU(int r,int g,int b) {
            int u = (int)(-0.169 * r - 0.332 * g + 0.5 * b) + 128;
            u = this.Normalize(u,0,255);
            return u;
        }
        private int GetV(int r,int g,int b) {
            int v = (int)(0.5 * r - 0.419 * g - 0.081 * b) + 128;
            v = this.Normalize(v,0,255);
            return v;
        }
        #endregion

        #region YUV to RGB
        private int GetR(int y,int u,int v) {
            int r = (int)(y + 1.402 * v);
            r = this.Normalize(r,0,255);
            return r;
        }
        private int GetG(int y,int u,int v) {
            int g = (int)(y - 0.344 * u - 0.714 * v);
            g = this.Normalize(g,0,255);
            return g;
        }
        private int GetB(int y,int u,int v) {
            int b = (int)(y + 1.772 * u);
            b = this.Normalize(b,0,255);
            return b;
        }
        #endregion

        #endregion

        #region Dark
        /// <summary>
        /// �w�肵�����l�ɂ���ĈÂ��Ȃ���YVColor�\���̂�Ԃ��܂��B
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public YUVColor Dark(int dy) {
            YUVColor c = new YUVColor(this.m_Y,this.m_U,this.m_V);
            c.m_Y -= dy;
            return c;
        }
        #endregion

        #region Light
        /// <summary>
        /// �w�肵�����l�ɂ���Ė��邭�Ȃ���YUVColor�\���̂�Ԃ��܂��B
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public YUVColor Light(int dy) {
            YUVColor c = new YUVColor(this.m_Y,this.m_U,this.m_V);
            c.m_Y += dy;
            return c;
        }
        #endregion

        #region Normalize
        private int Normalize(int value,int min,int max) {
            if(value < min) {
                value = min;
            }
            if(value > max) {
                value = max;
            }
            return value;
        }
        #endregion

        #region ToRGBColor
        /// <summary>
        /// ����YUVColor�\���̂ɑΉ�����RGBColor��Ԃ��܂��B
        /// </summary>
        /// <returns></returns>
        public Color ToRGBColor() {
            int y = this.m_Y;
            int u = this.m_U - 128;
            int v = this.m_V - 128;
            return Color.FromArgb(this.GetR(y,u,v),this.GetG(y,u,v),this.GetB(y,u,v));
        }
        #endregion

    }
    #endregion

}
