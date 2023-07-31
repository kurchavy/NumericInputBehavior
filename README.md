# NumericInputBehavior
WPF TextBox behavior for editing numbers (int or decimal).
<br><br>
```XAML
  OnlyIntegerInput="False" <!-- For decimals -->
  OnlyPositiveInput="False" <!-- Allow negative values -->
```

Usage (Requires Microsoft.Xaml.Behaviors):
```XAML
<UserControl ...
  xmlns:b="http://schemas.microsoft.com/xaml/behaviors">

<TextBox>
  <b:Interaction.Behaviors>
    <textBoxBehaviors:NumericInputBehavior OnlyIntegerInput="False" OnlyPositiveInput="False" />
  </b:Interaction.Behaviors>
</TextBox>
```
