#import "iOSNativeReview.h"
#import <StoreKit/StoreKit.h>

@implementation iOSNativeReview {
}

# pragma mark - C API

void requestReview() {
    NSLog(@"version %f", [UIDevice currentDevice].systemVersion.floatValue);
    if([UIDevice currentDevice].systemVersion.floatValue >= 10.3) {
        [SKStoreReviewController requestReview];
    }
}
@end
