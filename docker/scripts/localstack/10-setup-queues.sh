#!/bin/bash

# trade-imports-gmr-finder
aws --endpoint $LOCALSTACK_URL sns create-topic --region $AWS_REGION --name trade_imports_matched_gmrs
aws --endpoint $LOCALSTACK_URL sqs create-queue --region $AWS_REGION --queue-name trade_imports_matched_gmrs_processor
aws --endpoint $LOCALSTACK_URL sns subscribe --region $AWS_REGION --topic-arn arn:aws:sns:$AWS_REGION:000000000000:trade_imports_matched_gmrs --protocol sqs --notification-endpoint arn:aws:sqs:$AWS_REGION:000000000000:trade_imports_matched_gmrs_processor --attributes '{"RawMessageDelivery": "true"}'
aws --endpoint $LOCALSTACK_URL sqs create-queue --region $AWS_REGION --queue-name trade_imports_data_upserted_gmr_finder

# trade-imports-gmr-processor
aws --endpoint $LOCALSTACK_URL sqs create-queue --region $AWS_REGION --queue-name trade_imports_data_upserted_gmr_processor_gto
aws --endpoint $LOCALSTACK_URL sqs create-queue --region $AWS_REGION --queue-name trade_imports_matched_gmrs_processor_gto
aws --endpoint $LOCALSTACK_URL sqs create-queue --region $AWS_REGION --queue-name trade_imports_matched_gmrs_gmr_processor_eta
aws --endpoint $LOCALSTACK_URL sqs create-queue --region $AWS_REGION --queue-name trade_imports_matched_gmrs_gmr_processor_match

# list sns topics created
echo "Created the following sns topics:"
aws --endpoint $LOCALSTACK_URL sns --region $AWS_REGION list-topics

# list sqs queues created
echo "Created the following sqs queues:"
aws --endpoint $LOCALSTACK_URL sqs --region $AWS_REGION list-queues
