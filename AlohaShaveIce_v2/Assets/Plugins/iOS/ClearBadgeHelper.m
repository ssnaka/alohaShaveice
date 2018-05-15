//
//  ClearBadgeHelper.m
//  Unity-iPhone
//
//  Created by Antony Blackett

#import <Foundation/Foundation.h>

@interface ClearBadgeHelper : NSObject
+ (void) clearBadge:(NSNotification *)notification;
@end

@implementation ClearBadgeHelper

+ (void) clearBadge:(NSNotification *)notification
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
}

+ (void)load
{
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(clearBadge:) name:UIApplicationDidFinishLaunchingNotification object:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(clearBadge:) name:UIApplicationDidBecomeActiveNotification object:nil];
}

@end
