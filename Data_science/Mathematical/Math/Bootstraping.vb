﻿#Region "Microsoft.VisualBasic::b44c4ca21f91bccf205afa5f287d2aa7, ..\visualbasic_App\Data_science\Mathematical\Math\Bootstraping.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Mathematical.BasicR
Imports Microsoft.VisualBasic.Mathematical.SyntaxAPI.MathExtension
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

Public Module Bootstraping

    Public Function Sample(x%) As Vector
        Dim xvec As Integer() =
            New Random(Now.Millisecond).Permutation(x, x)
        Return New Vector(xvec.Select(Function(n) CDbl(n)))
    End Function

    ''' <summary>
    ''' bootstrap是一种非参数估计方法，它用到蒙特卡洛方法。bootstrap算法如下：
    ''' 假设样本容量为N
    '''
    ''' + 有放回的从样本中随机抽取N次(所以可能x1..xn中有的值会被抽取多次)，每次抽取一个元素。并将抽到的元素放到集合S中；
    ''' + 重复**步骤1** B次（例如``B = 100``）， 得到B个集合， 记作S1, S2,…, SB;
    ''' + 对每个Si （i=1,2,…,B），用蒙特卡洛方法估计随机变量的数字特征d，分别记作d1,d2,…,dB;
    ''' + 用d1,d2,…dB来近似d的分布；
    ''' 
    ''' 本质上，bootstrap算法是最大似然估计的一种实现，它和最大似然估计相比的优点在于，它不需要用参数来刻画总体分布。
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="N"></param>
    ''' <param name="B"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Samples(Of T)(source As IEnumerable(Of T), N As Integer, Optional B As Integer = 100) As IEnumerable(Of IntegerTagged(Of T()))
        Dim array As T() = source.ToArray
        Dim rnd As New Random(Now.Millisecond)

        For i As Integer = 0 To B
            Dim ls As New List(Of T)

            For k As Integer = 0 To N - 1
                ls += array(rnd.Next(array.Length))
            Next

            Yield New IntegerTagged(Of T()) With {
                .Tag = i,
                .value = ls.ToArray
            }
        Next
    End Function

    <Extension>
    Public Iterator Function Sampling(source As IEnumerable(Of Double), N As Integer, Optional B As Integer = 100) As IEnumerable(Of IntegerTagged(Of Vector))
        For Each x In Samples(source, N, B)
            Yield New IntegerTagged(Of Vector) With {
                .Tag = x.Tag,
                .value = New Vector(x.value)
            }
        Next
    End Function

    <Extension>
    Public Function Samples(Of T)(source As IEnumerable(Of T), getValue As Func(Of T, Double), N As Integer, Optional B As Integer = 100) As IEnumerable(Of IntegerTagged(Of Vector))
        Return source.Select(getValue).Sampling(N, B)
    End Function

    '' rcpp_trunc_ndist
    ''
    '' Truncated normal distribution (mean 1, respective upper and lower limits of
    '' 0 and 2).
    ''
    '' @param len Number of elements to be simulated
    '' @param sd Standard deviation
    ''
    '' @return A vector of truncated normally distributed values
    ''
    ' [[Rcpp::export]]
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="len"></param>
    ''' <param name="sd"></param>
    ''' <returns></returns>
    ''' <remarks>https://github.com/mpadge/tnorm</remarks>
    Public Function TruncNDist(len%, sd#) As Vector
        Dim eps As Vector ' Set up truncated normal distribution
        Dim z As New List(Of Double)()

        While z.Count < len
            eps = Normal.rnorm(len, 1.0, sd)
            For Each it As Double In eps
                If it >= 0.0 AndAlso it <= 2.0 Then
                    z.Add(it)
                End If
                it += 1
            Next
        End While

        Return New Vector(z)
    End Function

    ''' <summary>
    ''' 标准正态分布, delta = 1, u = 0
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    Public Function StandardDistribution#(x#)
        Dim answer As Double = 1 / ((Math.Sqrt(2 * Math.PI)))
        Dim exp1 As Double = Math.Pow(x, 2) / 2
        Dim exp As Double = Math.Pow(Math.E, -(exp1))
        answer = answer * exp
        Return answer
    End Function

    ''' <summary>
    ''' Normal Distribution.(正态分布)
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="m">Mean</param>
    ''' <param name="sd"></param>
    ''' <returns></returns>
    Public Function ProbabilityDensity(x#, m#, sd#) As Double
        Dim answer As Double = 1 / (sd * (Math.Sqrt(2 * Math.PI)))
        Dim exp As Double = Math.Pow((x - m), 2.0)
        Dim expP2 As Double = 2 * Math.Pow(sd, 2.0)
        Dim expP3 As Double = Math.Pow(Math.E, (-(exp / expP2)))
        answer = answer * expP3
        Return answer
    End Function

    Public Function AboveStandardDistribution(upperX As Double, n As Double, m As Double, sd As Double) As Double
        Dim lowerX As Double = m - 4.1 * sd
        Dim answer As Double = TrapezodialRule(lowerX, upperX, n, m, sd)
        Return 1 - answer
    End Function

    Public Function BelowStandardDistribution(upperX As Double, n As Double, m As Double, sd As Double) As Double
        Dim lowerX As Double = m + 4.1 * sd
        Dim answer As Double = TrapezodialRule(lowerX, upperX, n, m, sd)
        Return 1 + answer 'lol
    End Function

    Public Function BetweenStandardDistribution(lowerX As Double, upperX As Double, n As Double, m As Double, sd As Double) As Double
        Dim answer As Double = TrapezodialRule(lowerX, upperX, n, m, sd)
        Return answer
    End Function

    Public Function OutsideStandardDistribution(lowerX As Double, upperX As Double, n As Double, m As Double, sd As Double) As Double
        Dim answer As Double = 1 - TrapezodialRule(lowerX, upperX, n, m, sd)
        Return answer
    End Function

    Public Function TrapezodialRule(a#, b#, n#, m#, sd#) As Double
        Dim changeX As Double = (b - a) / n
        Dim a1 As Double = ProbabilityDensity(a, m, sd)
        Dim b1 As Double = ProbabilityDensity(b, m, sd)
        Dim c As Double = 0.5 * (a1 + b1)

        For i As Double = 1 To n - 1
            c = c + ProbabilityDensity((a + (i * changeX)), m, sd)
        Next i
        c = changeX * c

        Return c
    End Function

    Public Function Z#(x#, m#, sd#)
        Dim answer As Double = (x - m) / sd
        Return answer
    End Function

    ''' <summary>
    ''' 返回来的标签数据之中的标签是在某个区间范围内的数值集合的平均值
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="base"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Distributes(data As IEnumerable(Of Double), Optional base! = 10.0F) As Dictionary(Of Integer, DoubleTagged(Of Integer))
        Dim array As DoubleTagged(Of Double)() = data.ToArray(
            Function(x) New DoubleTagged(Of Double) With {
                .Tag = Math.Log(x, base),
                .value = x
            })
        Dim min As Integer = CInt(array.Min(Function(x) x.Tag)) - 1
        Dim max As Integer = CInt(array.Max(Function(x) x.Tag)) + 1
        Dim l As int = min, low As Integer = min
        Dim out As New Dictionary(Of Integer, DoubleTagged(Of Integer))

        Do While ++l < max
            Dim LQuery As DoubleTagged(Of Double)() =
                LinqAPI.Exec(Of DoubleTagged(Of Double)) <=
 _
                From x As DoubleTagged(Of Double)
                In array
                Where x.Tag >= low AndAlso
                    x.Tag < l
                Select x

            out(l) = New DoubleTagged(Of Integer) With {
                .Tag = If(LQuery.Length = 0, 0, LQuery.Average(Function(x) x.value)),
                .value = LQuery.Length
            }
            low = l
        Loop

        If out(min + 1).value = 0 Then
            Call out.Remove(min)
        End If
        If out(max - 1).value = 0 Then
            Call out.Remove(max)
        End If

        Return out
    End Function
End Module

