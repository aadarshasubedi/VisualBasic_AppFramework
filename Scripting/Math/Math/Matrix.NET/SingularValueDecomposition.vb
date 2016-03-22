Imports System.Runtime.Serialization

''' <summary>Singular Value Decomposition.
''' <P>
''' For an m-by-n matrix A with m >= n, the singular value decomposition is
''' an m-by-n orthogonal matrix U, an n-by-n diagonal matrix S, and
''' an n-by-n orthogonal matrix V so that A = U*S*V'.</P>
''' <P>
''' The singular values, sigma[k] = S[k][k], are ordered so that
''' sigma[0] >= sigma[1] >= ... >= sigma[n-1].</P>
''' <P>
''' The singular value decompostion always exists, so the constructor will
''' never fail.  The matrix condition number and the effective numerical
''' rank can be computed from this decomposition.</P>
''' </summary>
<Serializable>
Public Class SingularValueDecomposition
    Implements ISerializable

#Region "Class variables"

    ''' <summary>Arrays for internal storage of U and V.
    ''' @serial internal storage of U.
    ''' @serial internal storage of V.
    ''' </summary>
    Private U As Double()(), V As Double()()

	''' <summary>Array for internal storage of singular values.
	''' @serial internal storage of singular values.
	''' </summary>
	Private m_s As Double()

	''' <summary>Row and column dimensions.
	''' @serial row dimension.
	''' @serial column dimension.
	''' </summary>
	Private m As Integer, n As Integer

#End Region

#Region "Constructor"

    ''' <summary>
    ''' Construct the singular value decomposition, returns Structure to access U, S and V.
    ''' </summary>
    ''' <param name="Arg">   Rectangular matrix
    ''' </param>
    Public Sub New(Arg As GeneralMatrix)
        ' Derived from LINPACK code.
        ' Initialize.
        Dim A As Double()() = Arg.ArrayCopy
        m = Arg.RowDimension
        n = Arg.ColumnDimension
        Dim nu As Integer = System.Math.Min(m, n)
        m_s = New Double(System.Math.Min(m + 1, n) - 1) {}
        U = New Double(m - 1)() {}
        For i As Integer = 0 To m - 1
            U(i) = New Double(nu - 1) {}
        Next
        V = New Double(n - 1)() {}
        For i2 As Integer = 0 To n - 1
            V(i2) = New Double(n - 1) {}
        Next
        Dim e As Double() = New Double(n - 1) {}
        Dim work As Double() = New Double(m - 1) {}
        Dim wantu As Boolean = True
        Dim wantv As Boolean = True

        ' Reduce A to bidiagonal form, storing the diagonal elements
        ' in s and the super-diagonal elements in e.

        Dim nct As Integer = System.Math.Min(m - 1, n)
        Dim nrt As Integer = System.Math.Max(0, System.Math.Min(n - 2, m))
        For k As Integer = 0 To System.Math.Max(nct, nrt) - 1
            If k < nct Then

                ' Compute the transformation for the k-th column and
                ' place the k-th diagonal in s[k].
                ' Compute 2-norm of k-th column without under/overflow.
                m_s(k) = 0
                For i As Integer = k To m - 1
                    m_s(k) = Hypot(m_s(k), A(i)(k))
                Next
                If m_s(k) <> 0.0 Then
                    If A(k)(k) < 0.0 Then
                        m_s(k) = -m_s(k)
                    End If
                    For i As Integer = k To m - 1
                        A(i)(k) /= m_s(k)
                    Next
                    A(k)(k) += 1.0
                End If
                m_s(k) = -m_s(k)
            End If
            For j As Integer = k + 1 To n - 1
                If (k < nct) And (m_s(k) <> 0.0) Then

                    ' Apply the transformation.

                    Dim t As Double = 0
                    For i As Integer = k To m - 1
                        t += A(i)(k) * A(i)(j)
                    Next
                    t = (-t) / A(k)(k)
                    For i As Integer = k To m - 1
                        A(i)(j) += t * A(i)(k)
                    Next
                End If

                ' Place the k-th row of A into e for the
                ' subsequent calculation of the row transformation.

                e(j) = A(k)(j)
            Next
            If wantu And (k < nct) Then

                ' Place the transformation in U for subsequent back
                ' multiplication.

                For i As Integer = k To m - 1
                    U(i)(k) = A(i)(k)
                Next
            End If
            If k < nrt Then

                ' Compute the k-th row transformation and place the
                ' k-th super-diagonal in e[k].
                ' Compute 2-norm without under/overflow.
                e(k) = 0
                For i As Integer = k + 1 To n - 1
                    e(k) = Hypot(e(k), e(i))
                Next
                If e(k) <> 0.0 Then
                    If e(k + 1) < 0.0 Then
                        e(k) = -e(k)
                    End If
                    For i As Integer = k + 1 To n - 1
                        e(i) /= e(k)
                    Next
                    e(k + 1) += 1.0
                End If
                e(k) = -e(k)
                If (k + 1 < m) And (e(k) <> 0.0) Then

                    ' Apply the transformation.

                    For i As Integer = k + 1 To m - 1
                        work(i) = 0.0
                    Next
                    For j As Integer = k + 1 To n - 1
                        For i As Integer = k + 1 To m - 1
                            work(i) += e(j) * A(i)(j)
                        Next
                    Next
                    For j As Integer = k + 1 To n - 1
                        Dim t As Double = (-e(j)) / e(k + 1)
                        For i As Integer = k + 1 To m - 1
                            A(i)(j) += t * work(i)
                        Next
                    Next
                End If
                If wantv Then

                    ' Place the transformation in V for subsequent
                    ' back multiplication.

                    For i As Integer = k + 1 To n - 1
                        V(i)(k) = e(i)
                    Next
                End If
            End If
        Next

        ' Set up the final bidiagonal matrix or order p.

        Dim p As Integer = System.Math.Min(n, m + 1)
        If nct < n Then
            m_s(nct) = A(nct)(nct)
        End If
        If m < p Then
            m_s(p - 1) = 0.0
        End If
        If nrt + 1 < p Then
            e(nrt) = A(nrt)(p - 1)
        End If
        e(p - 1) = 0.0

        ' If required, generate U.

        If wantu Then
            For j As Integer = nct To nu - 1
                For i As Integer = 0 To m - 1
                    U(i)(j) = 0.0
                Next
                U(j)(j) = 1.0
            Next
            For k As Integer = nct - 1 To 0 Step -1
                If m_s(k) <> 0.0 Then
                    For j As Integer = k + 1 To nu - 1
                        Dim t As Double = 0
                        For i As Integer = k To m - 1
                            t += U(i)(k) * U(i)(j)
                        Next
                        t = (-t) / U(k)(k)
                        For i As Integer = k To m - 1
                            U(i)(j) += t * U(i)(k)
                        Next
                    Next
                    For i As Integer = k To m - 1
                        U(i)(k) = -U(i)(k)
                    Next
                    U(k)(k) = 1.0 + U(k)(k)
                    For i As Integer = 0 To k - 2
                        U(i)(k) = 0.0
                    Next
                Else
                    For i As Integer = 0 To m - 1
                        U(i)(k) = 0.0
                    Next
                    U(k)(k) = 1.0
                End If
            Next
        End If

        ' If required, generate V.

        If wantv Then
            For k As Integer = n - 1 To 0 Step -1
                If (k < nrt) And (e(k) <> 0.0) Then
                    For j As Integer = k + 1 To nu - 1
                        Dim t As Double = 0
                        For i As Integer = k + 1 To n - 1
                            t += V(i)(k) * V(i)(j)
                        Next
                        t = (-t) / V(k + 1)(k)
                        For i As Integer = k + 1 To n - 1
                            V(i)(j) += t * V(i)(k)
                        Next
                    Next
                End If
                For i As Integer = 0 To n - 1
                    V(i)(k) = 0.0
                Next
                V(k)(k) = 1.0
            Next
        End If

        ' Main iteration loop for the singular values.

        Dim pp As Integer = p - 1
        Dim iter As Integer = 0
        Dim eps As Double = System.Math.Pow(2.0, -52.0)
        While p > 0
            Dim k As Integer, kase As Integer

            ' Here is where a test for too many iterations would go.

            ' This section of the program inspects for
            ' negligible elements in the s and e arrays.  On
            ' completion the variables kase and k are set as follows.

            ' kase = 1     if s(p) and e[k-1] are negligible and k<p
            ' kase = 2     if s(k) is negligible and k<p
            ' kase = 3     if e[k-1] is negligible, k<p, and
            '              s(k), ..., s(p) are not negligible (qr step).
            ' kase = 4     if e(p-1) is negligible (convergence).

            For k = p - 2 To -1 Step -1
                If k = -1 Then
                    Exit For
                End If
                If System.Math.Abs(e(k)) <= eps * (System.Math.Abs(m_s(k)) + System.Math.Abs(m_s(k + 1))) Then
                    e(k) = 0.0
                    Exit For
                End If
            Next
            If k = p - 2 Then
                kase = 4
            Else
                Dim ks As Integer
                For ks = p - 1 To k Step -1
                    If ks = k Then
                        Exit For
                    End If
                    Dim t As Double = (If(ks <> p, System.Math.Abs(e(ks)), 0.0)) + (If(ks <> k + 1, System.Math.Abs(e(ks - 1)), 0.0))
                    If System.Math.Abs(m_s(ks)) <= eps * t Then
                        m_s(ks) = 0.0
                        Exit For
                    End If
                Next
                If ks = k Then
                    kase = 3
                ElseIf ks = p - 1 Then
                    kase = 1
                Else
                    kase = 2
                    k = ks
                End If
            End If
            k += 1

            ' Perform the task indicated by kase.

            Select Case kase


                ' Deflate negligible s(p).
                Case 1
                    If True Then
                        Dim f As Double = e(p - 2)
                        e(p - 2) = 0.0
                        For j As Integer = p - 2 To k Step -1
                            Dim t As Double = Hypot(m_s(j), f)
                            Dim cs As Double = m_s(j) / t
                            Dim sn As Double = f / t
                            m_s(j) = t
                            If j <> k Then
                                f = (-sn) * e(j - 1)
                                e(j - 1) = cs * e(j - 1)
                            End If
                            If wantv Then
                                For i As Integer = 0 To n - 1
                                    t = cs * V(i)(j) + sn * V(i)(p - 1)
                                    V(i)(p - 1) = (-sn) * V(i)(j) + cs * V(i)(p - 1)
                                    V(i)(j) = t
                                Next
                            End If
                        Next
                    End If
                    Exit Select

                ' Split at negligible s(k).


                Case 2
                    If True Then
                        Dim f As Double = e(k - 1)
                        e(k - 1) = 0.0
                        For j As Integer = k To p - 1
                            Dim t As Double = Hypot(m_s(j), f)
                            Dim cs As Double = m_s(j) / t
                            Dim sn As Double = f / t
                            m_s(j) = t
                            f = (-sn) * e(j)
                            e(j) = cs * e(j)
                            If wantu Then
                                For i As Integer = 0 To m - 1
                                    t = cs * U(i)(j) + sn * U(i)(k - 1)
                                    U(i)(k - 1) = (-sn) * U(i)(j) + cs * U(i)(k - 1)
                                    U(i)(j) = t
                                Next
                            End If
                        Next
                    End If
                    Exit Select

                ' Perform one qr step.


                Case 3
                    If True Then
                        ' Calculate the shift.

                        Dim scale As Double = System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Max(System.Math.Abs(m_s(p - 1)), System.Math.Abs(m_s(p - 2))), System.Math.Abs(e(p - 2))), System.Math.Abs(m_s(k))), System.Math.Abs(e(k)))
                        Dim sp As Double = m_s(p - 1) / scale
                        Dim spm1 As Double = m_s(p - 2) / scale
                        Dim epm1 As Double = e(p - 2) / scale
                        Dim sk As Double = m_s(k) / scale
                        Dim ek As Double = e(k) / scale
                        Dim b As Double = ((spm1 + sp) * (spm1 - sp) + epm1 * epm1) / 2.0
                        Dim c As Double = (sp * epm1) * (sp * epm1)
                        Dim shift As Double = 0.0
                        If (b <> 0.0) Or (c <> 0.0) Then
                            shift = System.Math.Sqrt(b * b + c)
                            If b < 0.0 Then
                                shift = -shift
                            End If
                            shift = c / (b + shift)
                        End If
                        Dim f As Double = (sk + sp) * (sk - sp) + shift
                        Dim g As Double = sk * ek

                        ' Chase zeros.

                        For j As Integer = k To p - 2
                            Dim t As Double = Hypot(f, g)
                            Dim cs As Double = f / t
                            Dim sn As Double = g / t
                            If j <> k Then
                                e(j - 1) = t
                            End If
                            f = cs * m_s(j) + sn * e(j)
                            e(j) = cs * e(j) - sn * m_s(j)
                            g = sn * m_s(j + 1)
                            m_s(j + 1) = cs * m_s(j + 1)
                            If wantv Then
                                For i As Integer = 0 To n - 1
                                    t = cs * V(i)(j) + sn * V(i)(j + 1)
                                    V(i)(j + 1) = (-sn) * V(i)(j) + cs * V(i)(j + 1)
                                    V(i)(j) = t
                                Next
                            End If
                            t = Hypot(f, g)
                            cs = f / t
                            sn = g / t
                            m_s(j) = t
                            f = cs * e(j) + sn * m_s(j + 1)
                            m_s(j + 1) = (-sn) * e(j) + cs * m_s(j + 1)
                            g = sn * e(j + 1)
                            e(j + 1) = cs * e(j + 1)
                            If wantu AndAlso (j < m - 1) Then
                                For i As Integer = 0 To m - 1
                                    t = cs * U(i)(j) + sn * U(i)(j + 1)
                                    U(i)(j + 1) = (-sn) * U(i)(j) + cs * U(i)(j + 1)
                                    U(i)(j) = t
                                Next
                            End If
                        Next
                        e(p - 2) = f
                        iter = iter + 1
                    End If
                    Exit Select

                ' Convergence.


                Case 4
                    If True Then
                        ' Make the singular values positive.

                        If m_s(k) <= 0.0 Then
                            m_s(k) = (If(m_s(k) < 0.0, -m_s(k), 0.0))
                            If wantv Then
                                For i As Integer = 0 To pp
                                    V(i)(k) = -V(i)(k)
                                Next
                            End If
                        End If

                        ' Order the singular values.

                        While k < pp
                            If m_s(k) >= m_s(k + 1) Then
                                Exit While
                            End If
                            Dim t As Double = m_s(k)
                            m_s(k) = m_s(k + 1)
                            m_s(k + 1) = t
                            If wantv AndAlso (k < n - 1) Then
                                For i As Integer = 0 To n - 1
                                    t = V(i)(k + 1)
                                    V(i)(k + 1) = V(i)(k)
                                    V(i)(k) = t
                                Next
                            End If
                            If wantu AndAlso (k < m - 1) Then
                                For i As Integer = 0 To m - 1
                                    t = U(i)(k + 1)
                                    U(i)(k + 1) = U(i)(k)
                                    U(i)(k) = t
                                Next
                            End If
                            k += 1
                        End While
                        iter = 0
                        p -= 1
                    End If
                    Exit Select
            End Select
        End While
    End Sub
#End Region

#Region "Public Properties"
    ''' <summary>Return the one-dimensional array of singular values</summary>
    ''' <returns>     diagonal of S.
    ''' </returns>
    Public Overridable ReadOnly Property SingularValues() As Double()
		Get
			Return m_s
		End Get
	End Property

	''' <summary>Return the diagonal matrix of singular values</summary>
	''' <returns>     S
	''' </returns>
	Public Overridable ReadOnly Property S() As GeneralMatrix
		Get
			Dim X As New GeneralMatrix(n, n)
            Dim Sa As Double()() = X.Array
            For i As Integer = 0 To n - 1
				For j As Integer = 0 To n - 1
                    Sa(i)(j) = 0.0
                Next
                Sa(i)(i) = Me.m_s(i)
            Next
			Return X
		End Get
	End Property
	#End Region

	#Region "Public Methods"

	''' <summary>Return the left singular vectors</summary>
	''' <returns>     U
	''' </returns>

	Public Overridable Function GetU() As GeneralMatrix
		Return New GeneralMatrix(U, m, System.Math.Min(m + 1, n))
	End Function

	''' <summary>Return the right singular vectors</summary>
	''' <returns>     V
	''' </returns>

	Public Overridable Function GetV() As GeneralMatrix
		Return New GeneralMatrix(V, n, n)
	End Function

	''' <summary>Two norm</summary>
	''' <returns>     max(S)
	''' </returns>

	Public Overridable Function Norm2() As Double
		Return m_s(0)
	End Function

	''' <summary>Two norm condition number</summary>
	''' <returns>     max(S)/min(S)
	''' </returns>

	Public Overridable Function Condition() As Double
		Return m_s(0) / m_s(System.Math.Min(m, n) - 1)
	End Function

	''' <summary>Effective numerical matrix rank</summary>
	''' <returns>     Number of nonnegligible singular values.
	''' </returns>

	Public Overridable Function Rank() As Integer
		Dim eps As Double = System.Math.Pow(2.0, -52.0)
		Dim tol As Double = System.Math.Max(m, n) * m_s(0) * eps
		Dim r As Integer = 0
		For i As Integer = 0 To m_s.Length - 1
			If m_s(i) > tol Then
				r += 1
			End If
		Next
		Return r
	End Function
	#End Region

	' A method called when serializing this class.
	Private Sub ISerializable_GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
	End Sub
End Class
