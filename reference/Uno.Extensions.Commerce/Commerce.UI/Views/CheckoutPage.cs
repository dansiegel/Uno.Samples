namespace Commerce.Views;

public partial class CheckoutPage : Page
{
	public CheckoutPage()
	{
		this.Background(_ => _.StaticResource(StaticResources.BackgroundBrush))
			.Content(new Grid()
				.MinHeight(300)
				.MinWidth(300)
				.RowDefinitions("Auto", "*")
				.Children(
					new NavigationBar()
						.Content("Checkout"),
					new StackPanel()
						.Grid(row: 1)
						.Children(
							new TextBlock()
								.Text("Checkout")
								.FontSize(32)
								.HorizontalAlignment(HorizontalAlignment.Center)
								.VerticalAlignment(VerticalAlignment.Center),
							new Button()
								.Content("Done")
								.Navigation(request: "/-")
						)
				)
			);
	}
}
