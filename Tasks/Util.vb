Module Util

  <Extension>
  Public Function ItemOrDefault(
    Of TKey, TValue
    )(
    dictionary As IDictionary(Of TKey, TValue),
    key As TKey
    ) As TValue

    Try
      Return dictionary.Item(key)
    Catch ex As KeyNotFoundException
      Return Nothing
    End Try
  End Function

End Module
