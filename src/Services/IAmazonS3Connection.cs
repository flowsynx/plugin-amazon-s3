﻿using Amazon.S3;
using FlowSynx.Plugins.Amazon.S3.Models;

namespace FlowSynx.Plugins.Amazon.S3.Services;

internal interface IAmazonS3Connection
{
    AmazonS3Client Connect(AmazonS3Specifications specifications);
}