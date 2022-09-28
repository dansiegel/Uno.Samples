using Uno.Extensions.Navigation.UI;

namespace Commerce.Views;

public partial class CartFlyout : Flyout
{
	public CartFlyout()
	{
		this.Placement(FlyoutPlacementMode.Full)
			.FlyoutPresenterStyle(x => x.StaticResource(StyleNames.FlyoutPresenterStyle))
			.Content(new Grid()
				.Children(
					new Frame()
						.Region(attached: true)
						.HorizontalAlignment(HorizontalAlignment.Stretch)
						.VerticalAlignment(VerticalAlignment.Stretch)
						.HorizontalContentAlignment(HorizontalAlignment.Stretch)
						.VerticalContentAlignment(VerticalAlignment.Stretch)
				)
			);
	}
}
