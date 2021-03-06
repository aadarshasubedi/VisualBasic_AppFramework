﻿#Region "Microsoft.VisualBasic::38123610a59dc41d14c2a0d541e9ebe7, ..\visualbasic_App\Data_science\Mathematical\ODE\ODE.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Ordinary differential equation(ODE).(常微分方程的模型)
''' </summary>
Public Class ODE

#Region "Output results"

    Public Property x As Double()
    Public Property y As Double()
#End Region

    ''' <summary>
    ''' Public Delegate Function df(x As Double, y As Double) As Double
    ''' (从这里传递自定义的函数指针)
    ''' </summary>
    ''' <returns></returns>
    Public Property df As ODESolver.df
    ''' <summary>
    ''' ``x=a``的时候的y初始值
    ''' </summary>
    ''' <returns></returns>
    Public Property y0 As Double

    Default Public ReadOnly Property GetValue(xi As Double, yi As Double) As Double
        Get
            Return _df(xi, yi)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return x.GetJson & " --> " & y.GetJson
    End Function
End Class
