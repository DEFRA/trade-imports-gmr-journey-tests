namespace TradeImportsGmr.JourneyTests.ETA;

public class UpdatedTimeOfArrivalTest
{
    [Fact]
    public void TrueIsTrue()
    {
        Assert.True(true);
    }
}

// send message to input sqs queue trade_imports_data_upserted_gmr_finder
// assert message gets to output sns topic trade_imports_matched_gmrs_gmr_processor_eta (don't forget to match on gvms stub)

// ETA - Provides an updated time of arrival to Ipaffs when a GMR is marked as Embarked
